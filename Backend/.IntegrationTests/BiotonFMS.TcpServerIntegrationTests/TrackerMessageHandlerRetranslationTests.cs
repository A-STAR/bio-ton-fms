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
public sealed class TrackerMessageHandlerRetranslationTests : IAsyncLifetime
{
    private readonly IContainer _trackerMessageHandlerContainer;
    private readonly IContainer _tcpServerContainer;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly RabbitMQMessageBus _rawMessageBus;
    private readonly RabbitMQMessageBus _rawMessageRetranslatorBus;
    private string _externalMessageHandlerConfigName;
    private List<byte[]> receivedMessages = new List<byte[]>();
    private List<byte[]> receivedTcpServerMessages = new List<byte[]>();

    public TrackerMessageHandlerRetranslationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        _externalMessageHandlerConfigName = @"appsettings.messagehandler.external.json";
        var externalTcpServerConfigName = @"appsettings.trackertcpserver.external.json";
        var containerConfigName = @"appsettings.messagehandler.container.json";
        var containerTcpServerConfigName = @"appsettings.trackertcpserver.container.json";

        string? ciEnvironment = Environment.GetEnvironmentVariable("CiEnv");

        _testOutputHelper.WriteLine($"ciEnvironment = {ciEnvironment}");
        if (ciEnvironment is not null)
        {
            _externalMessageHandlerConfigName = @"appsettings.messagehandler.ci-external.json";
            externalTcpServerConfigName = @"appsettings.trackertcpserver.ci-external.json";
            containerConfigName = @"appsettings.messagehandler.ci-container.json";
            containerTcpServerConfigName = @"appsettings.trackertcpserver.ci-container.json";
        }

        var logger = new TestOutputHelperLoggerMock<RabbitMQMessageBus>(_testOutputHelper);
        

        RabbitMQOptions messageHandlerRabbitMqSettings = GetRabbitMQOptions("MessageHandlerRetranslationTests", _externalMessageHandlerConfigName);
        var messageHandlerRabbitMqOptions = Options.Create(messageHandlerRabbitMqSettings);
        var mhServiceProvider = new Mock<IServiceProvider>();
        _rawMessageBus = new RabbitMQMessageBus(logger, mhServiceProvider.Object, messageHandlerRabbitMqOptions, isDurable: true, messageHandlerRabbitMqOptions.Value.RawMessageQueueName,
            needDeadMessageQueue: true, deliveryLimit: messageHandlerRabbitMqSettings.DeliveryLimit, queueMaxLength: messageHandlerRabbitMqSettings.TrackerQueueMaxLength);
        mhServiceProvider.Setup(s => s.GetService(typeof(TestTrackerMessageHandlerMessageHandler))).Returns(new TestTrackerMessageHandlerRetranslationMessageHandler(_rawMessageBus, receivedMessages));

        RabbitMQOptions tcpServerRabbitMqSettings = GetRabbitMQOptions("MessageHandlerRetranslationTests", externalTcpServerConfigName);
        var tcpServerRabbitMqOptions = Options.Create(tcpServerRabbitMqSettings);
        var tsServiceProvider = new Mock<IServiceProvider>();
        _rawMessageRetranslatorBus = new RabbitMQMessageBus(logger, tsServiceProvider.Object, tcpServerRabbitMqOptions, isDurable: true, tcpServerRabbitMqOptions.Value.RawMessageQueueName,
            needDeadMessageQueue: true, deliveryLimit: messageHandlerRabbitMqSettings.DeliveryLimit, queueMaxLength: messageHandlerRabbitMqSettings.TrackerQueueMaxLength);
        tsServiceProvider.Setup(s => s.GetService(typeof(TestTcpTrackerServerMessageHandler))).Returns(new TestTcpTrackerServerMessageHandler(_rawMessageRetranslatorBus, receivedTcpServerMessages));

        string? imageTag = Environment.GetEnvironmentVariable("DOCKER_TAG");
        if (imageTag is null)
        {
            imageTag = "latest";
        }
        _testOutputHelper.WriteLine($"imageTag = {imageTag}");

        
        byte[] appSettings = GetAppsettingsFile("MessageHandlerRetranslationTests", containerConfigName);

        var trackerMessageHandlerContainerBuilder = new ContainerBuilder()
                    .WithImage($"fms-tracker-message:{imageTag}")
                    .WithResourceMapping(appSettings, "/app/config/appsettings.json");
        if (ciEnvironment is null)
        {
            trackerMessageHandlerContainerBuilder = trackerMessageHandlerContainerBuilder.WithNetwork("integration-tests");
        }
        _trackerMessageHandlerContainer = trackerMessageHandlerContainerBuilder.Build();

        byte[] tcpServerAppSettings = GetAppsettingsFile("MessageHandlerRetranslationTests", containerTcpServerConfigName);

        var tcpServerContainerBuilder = new ContainerBuilder()
                    .WithImage($"fms-tracker-tcp-server:{imageTag}")
                    .WithName("tcp-server")
                    .WithResourceMapping(tcpServerAppSettings, "/app/config/appsettings.json")
                    .WithPortBinding(6000, false)
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6000));
        if (ciEnvironment is null)
        {
            tcpServerContainerBuilder = tcpServerContainerBuilder.WithNetwork("integration-tests");
        }
        _tcpServerContainer = tcpServerContainerBuilder.Build();
    }

    private MessagesDBContext CreateMsgContext(string externalConfigName)
    {
        var msgConnectionString = GetConnectionstring("MessageHandlerRetranslationTests", externalConfigName, "MessagesConnection");
        MessagesDBContext msgDbContext = new MessagesDBContext(new DbContextOptionsBuilder<MessagesDBContext>()
                .UseNpgsql(msgConnectionString)
                .UseSnakeCaseNamingConvention()
                .Options);
        return msgDbContext;
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _tcpServerContainer.StartAsync();
        }
        catch 
        {
            var logs = await _tcpServerContainer.GetLogsAsync();
            _testOutputHelper.WriteLine("Container failed to start _tcpServerContainer - stdout:");
            _testOutputHelper.WriteLine(logs.Stdout);

            _testOutputHelper.WriteLine("Container failed to start _tcpServerContainer - stderr:");
            _testOutputHelper.WriteLine(logs.Stderr);
            throw;
        }
        try
        { 
            _testOutputHelper.WriteLine("_tcpServerContainer.Hostname " + _tcpServerContainer.Hostname);
            _testOutputHelper.WriteLine("_tcpServerContainer.IpAddress " + _tcpServerContainer.IpAddress);
            _testOutputHelper.WriteLine("_tcpServerContainer.GetMappedPublicPort(6000) " + _tcpServerContainer.GetMappedPublicPort(6000)); 
            await _trackerMessageHandlerContainer.StartAsync();
        }
        catch
        {
            var logs = await _trackerMessageHandlerContainer.GetLogsAsync();
            _testOutputHelper.WriteLine("Container failed to start _trackerMessageHandlerContainer - stdout:");
            _testOutputHelper.WriteLine(logs.Stdout);

            _testOutputHelper.WriteLine("Container failed to start _trackerMessageHandlerContainer - stderr:");
            _testOutputHelper.WriteLine(logs.Stderr);
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        await _tcpServerContainer.DisposeAsync();
        await _trackerMessageHandlerContainer.DisposeAsync();
    }

    [Fact]
    public async void TrackerMessageHandler_ShouldProcessMessageAndRetranslateIt()
    {
        // Задержка для того, чтобы контейнеры успели инициализироваться
        CancellationToken stoppingToken = new CancellationToken();
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

        // очистим очереди, принимающие участие в тесте
        _rawMessageBus.PurgeQueue();
        _rawMessageRetranslatorBus.PurgeQueue();

        // подпишемся на сообщения из очереди TcpServer, получающего ретранслированные сообщения
        // это позволит проверить, приходят ли к нему ретранслируемые сообщения
        _rawMessageRetranslatorBus.Subscribe<TestTcpTrackerServerMessageHandler>();

        // подготовим сообщение для отправки в MessageHandler
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
        // Опубликуем сообщение. 
        var publishNumber = _rawMessageBus.Publish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rawTrackerMessage)));
        _testOutputHelper.LogInformation("Сообщение отправлено. SeqNum = {0} Len = {1} PackageUID = {2}", publishNumber, rawMessage.Length, rawTrackerMessage.PackageUID);

        // MessageHandler должен принять опубликованное сообщение, разобрать, положить в БД и ретранслировать его 

        // дадим гарантированный запас времени на обработку    
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        // Выведем логи контейнеров, учавствовавших в тесте
        var logs = await _tcpServerContainer.GetLogsAsync();
        _testOutputHelper.WriteLine("_tcpServerContainer  stdout:");
        _testOutputHelper.WriteLine(logs.Stdout);

        _testOutputHelper.WriteLine("_tcpServerContainer stderr:");
        _testOutputHelper.WriteLine(logs.Stderr);

        logs = await _trackerMessageHandlerContainer.GetLogsAsync();
        _testOutputHelper.WriteLine("_trackerMessageHandlerContainer stdout:");
        _testOutputHelper.WriteLine(logs.Stdout);

        _testOutputHelper.WriteLine("_trackerMessageHandlerContainer stderr:");
        _testOutputHelper.WriteLine(logs.Stderr);

        // Проверим, что тест отработал корректно

        // Отправленное сообщение обработано и положено в БД
        using var msgDbContext = CreateMsgContext(_externalMessageHandlerConfigName);
        msgDbContext.TrackerMessages.Where(m => m.PackageUID == rawTrackerMessage.PackageUID).Count().Should().Be(1);
        var messageTags = msgDbContext.TrackerMessages.Where(m => m.PackageUID == rawTrackerMessage.PackageUID)
            .SelectMany(m => m.Tags).ToArray();
        messageTags.Where(t => t.ValueString == "862057047832826").ToList().Should().HaveCount(1);

        // TcpServer на который бало отправлено ретранслированное сообщение получил его и положил в очередь
        byte[] messageToCheck = TestUtils.ReadMessageFromFile("TestData/11-package.txt");
        receivedTcpServerMessages.Count().Should().Be(1);
        var receivedRawTrackerMessage = GetTrackerRawMessage(receivedTcpServerMessages.First());
        receivedRawTrackerMessage.Should().BeEquivalentTo(messageToCheck);
    }

    private byte[] GetTrackerRawMessage(byte[] binaryMessage)
    {
        var messageText = Encoding.UTF8.GetString(binaryMessage);

        RawTrackerMessage? rawTrackerMessage = JsonSerializer.Deserialize<RawTrackerMessage>(messageText);
        if (rawTrackerMessage is not null)
        {
            return rawTrackerMessage.RawMessage;
        }
        return new List<byte>().ToArray();
    }

    private byte[] GetAppsettingsFile(string testSetName, string settingsFileName)
    {
        var type = GetType();
        var name = $@"configs.{testSetName}.{settingsFileName}";

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

    private RabbitMQOptions GetRabbitMQOptions(string testSetName, string settingsFileName)
    {
        var type = GetType();
        var name = @$"configs.{testSetName}.{settingsFileName}";
        var rabbitMQOptions = new RabbitMQOptions();
        using (var configStream = type.Assembly.GetManifestResourceStream(type, name))
        {
            var config = new ConfigurationBuilder().AddJsonStream(configStream).Build();
            config.Bind("RabbitMQ", rabbitMQOptions);
        }
        return rabbitMQOptions;
    }

    private string GetConnectionstring(string testSetName, string settingsFileName, string connectionOption)
    {
        var type = GetType();
        var name = @$"configs.{testSetName}.{settingsFileName}";
        var connectionString = string.Empty;
        using (var configStream = type.Assembly.GetManifestResourceStream(type, name))
        {
            var config = new ConfigurationBuilder().AddJsonStream(configStream).Build();
            connectionString = config.GetConnectionString(connectionOption);
        }
        return connectionString;
    }
}

public class TestTrackerMessageHandlerRetranslationMessageHandler : IBusMessageHandler
{
    private readonly IMessageBus _consumerBus;
    private readonly List<byte[]> _receivedMessages;

    public TestTrackerMessageHandlerRetranslationMessageHandler(IMessageBus consumerBus,
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