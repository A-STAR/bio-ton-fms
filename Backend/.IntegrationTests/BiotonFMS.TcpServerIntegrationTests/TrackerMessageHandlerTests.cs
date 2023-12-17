using Xunit;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using Xunit.Abstractions;
using BioTonFMS.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BiotonFMS.IntegrationTests.Utils;
using BioTonFMS.Domain.Messaging;
using System.Text;
using System.Text.Json;
using BioTonFMS.Infrastructure.RabbitMQ;
using Moq;
using Microsoft.Extensions.Options;
using BioTonFMS.Infrastructure.MessageBus;
using BiotonFMS.IntegrationTests.Utils.Mocks;
using BioTonFMS.Domain;
using System.Net;
using BiotonFMS.IntegrationTests.Extensions;
using BioTonFMS.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace BiotonFMS.IntegrationTests;

[Collection("Sequential")]
public sealed class TrackerMessageHandlerTests : IAsyncLifetime
{
    private readonly IContainer _container;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly RabbitMQMessageBus _messageBus;
    private string _externalConfigName;
    private List<byte[]> receivedMessages = new List<byte[]>();

    public TrackerMessageHandlerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        _externalConfigName = @"appsettings.messagehandler.external.json";
        var containerConfigName = @"appsettings.messagehandler.container.json";

        string? ciEnvironment = Environment.GetEnvironmentVariable("CiEnv");

        _testOutputHelper.WriteLine($"ciEnvironment = {ciEnvironment}");
        if (ciEnvironment is not null)
        {
            _externalConfigName = @"appsettings.messagehandler.ci-external.json";
            containerConfigName = @"appsettings.messagehandler.ci-container.json";
        }

        RabbitMQOptions rabbitMqSettings = GetRabbitMQOptions(_externalConfigName);
        var logger = new TestOutputHelperLoggerMock<RabbitMQMessageBus>(_testOutputHelper);
        var serviceProvider = new Mock<IServiceProvider>();
        var rabbitMqOptions = Options.Create(rabbitMqSettings);
        _messageBus = new RabbitMQMessageBus(logger, serviceProvider.Object, rabbitMqOptions, isDurable: true, "RawTrackerMessages-primary",
            needDeadMessageQueue: true, deliveryLimit: rabbitMqSettings.DeliveryLimit, queueMaxLength: rabbitMqSettings.TrackerQueueMaxLength);
        serviceProvider.Setup(s => s.GetService(typeof(TestTrackerMessageHandlerMessageHandler))).Returns(new TestTrackerMessageHandlerMessageHandler(_messageBus, receivedMessages));

        string? imageTag = Environment.GetEnvironmentVariable("DOCKER_TAG");
        if (imageTag is null)
        {
            imageTag = "latest";
        }
        _testOutputHelper.WriteLine($"imageTag = {imageTag}");

        byte[] appSettings = GetAppsettingsFile(containerConfigName);

        _container = new ContainerBuilder()
                    .WithImage($"fms-tracker-message:{imageTag}")
                    .WithResourceMapping(appSettings, "/app/config/appsettings.json")
                    .WithPortBinding(6000, true)
                    //.WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Application started. Press Ctrl+C to shut down"))
                    // Build the _container configuration.
                    .Build();

    }

    private MessagesDBContext CreateMsgContext(string externalConfigName)
    {
        var msgConnectionString = GetConnectionstring(externalConfigName, "MessagesConnection");
        MessagesDBContext msgDbContext = new MessagesDBContext(new DbContextOptionsBuilder<MessagesDBContext>()
                .UseNpgsql(msgConnectionString)
                .UseSnakeCaseNamingConvention()
                .Options);
        return msgDbContext;
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [Fact]
    public async void TrackerMessageHandler_ShouldProcessMessageAndCreateMessageInDB()
    {
        CancellationToken stoppingToken = new CancellationToken();
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        _messageBus.PurgeQueue();

        var rawMessage = TestUtils.ReadMessageFromFile("TestData/11-package.txt");
        var testIp = IPAddress.Parse("111.222.111.222");
        int testPort = 12345;

        var rawTrackerMessage = new RawTrackerMessage
        {
            RawMessage = rawMessage,
            TrackerType = TrackerTypeEnum.GalileoSkyV50,
            PackageUID = Guid.NewGuid(),
            IpAddress = testIp.ToString(),
            Port = testPort
        };
        var publishNumber = _messageBus.Publish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rawTrackerMessage)));
        _testOutputHelper.LogInformation("Сообщение отправлено. SeqNum = {0} Len = {1} PackageUID = {2}", publishNumber, rawMessage.Length, rawTrackerMessage.PackageUID);

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        var logs = await _container.GetLogsAsync();
        _testOutputHelper.WriteLine("Container stdout:");
        _testOutputHelper.WriteLine(logs.Stdout);

        _testOutputHelper.WriteLine("Container stderr:");
        _testOutputHelper.WriteLine(logs.Stderr);

        using var msgDbContext = CreateMsgContext(_externalConfigName);

        msgDbContext.TrackerMessages.Where(m => m.PackageUID == rawTrackerMessage.PackageUID).Count().Should().Be(1);
        var messageTags = msgDbContext.TrackerMessages.Where(m => m.PackageUID == rawTrackerMessage.PackageUID)
            .SelectMany(m => m.Tags).ToArray();
        messageTags.Where(t => t.ValueString == "862057047832826").ToList().Should().HaveCount(1);


    }

    private byte[] GetAppsettingsFile(string settingsFileName)
    {
        var type = GetType();
        var name = @"configs.MessageHandlerTests." + settingsFileName;

        using (var configStream = type.Assembly.GetManifestResourceStream(type, name))
        {
            if (configStream != null)
            {
                using var stream = new MemoryStream();
                configStream.CopyTo(stream);
                return stream.ToArray();
            }
            throw new ArgumentException();
        }
    }

    private RabbitMQOptions GetRabbitMQOptions(string settingsFileName)
    {
        var type = GetType();
        var name = @"configs.MessageHandlerTests." + settingsFileName;
        var rabbitMQOptions = new RabbitMQOptions();
        using (var configStream = type.Assembly.GetManifestResourceStream(type, name))
        {
            var config = new ConfigurationBuilder().AddJsonStream(configStream).Build();
            config.Bind("RabbitMQ", rabbitMQOptions);
        }
        return rabbitMQOptions;
    }

    private string GetConnectionstring(string settingsFileName, string connectionOption)
    {
        var type = GetType();
        var name = @"configs.MessageHandlerTests." + settingsFileName;
        var connectionString = string.Empty;
        using (var configStream = type.Assembly.GetManifestResourceStream(type, name))
        {
            var config = new ConfigurationBuilder().AddJsonStream(configStream).Build();
            connectionString = config.GetConnectionString(connectionOption);
        }
        return connectionString;
    }
}

public class TestTrackerMessageHandlerMessageHandler : IBusMessageHandler
{
    private readonly IMessageBus _consumerBus;
    private readonly List<byte[]> _receivedMessages;

    public TestTrackerMessageHandlerMessageHandler(IMessageBus consumerBus,
        List<byte[]> receivedMessages)
    {
        _consumerBus = consumerBus;
        _receivedMessages = receivedMessages;
    }

    public Task HandleAsync(byte[] binaryMessage, ulong deliveryTag)
    {
        _receivedMessages.Add(binaryMessage);
        _consumerBus.Ack(deliveryTag, multiple: false);
        return Task.CompletedTask;
    }
}