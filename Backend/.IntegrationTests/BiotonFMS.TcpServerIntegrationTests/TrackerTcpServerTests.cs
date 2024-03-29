﻿using Xunit;
using System.Net.Sockets;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Builders;
using Xunit.Abstractions;
using BioTonFMS.Common.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BiotonFMS.IntegrationTests.Utils;
using FluentAssertions;
using BioTonFMS.Domain.Messaging;
using System.Text;
using System.Text.Json;
using BioTonFMS.Infrastructure.RabbitMQ;
using Moq;
using Microsoft.Extensions.Options;
using BioTonFMS.Infrastructure.MessageBus;
using BiotonFMS.IntegrationTests.Utils.Mocks;
using DotNet.Testcontainers.Networks;

namespace BiotonFMS.IntegrationTests;

[Collection("Sequential")]
public sealed class TrackerTcpServerTests : IAsyncLifetime
{
    private readonly IContainer _container;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly RabbitMQMessageBus _messageBus;
    private List<byte[]> receivedMessages = new List<byte[]>();

    public TrackerTcpServerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var externalConfigName = @"appsettings.trackertcpserver.external.json";
        var containerConfigName = @"appsettings.trackertcpserver.container.json";

        string? ciEnvironment = Environment.GetEnvironmentVariable("CiEnv");

        _testOutputHelper.WriteLine($"ciEnvironment = {ciEnvironment}");
        if (ciEnvironment is not null) 
        {
            externalConfigName = @"appsettings.trackertcpserver.ci-external.json";
            containerConfigName = @"appsettings.trackertcpserver.ci-container.json";
        }

        RabbitMQOptions rabbitMqSettings = GetRabbitMQOptions(externalConfigName);
        var logger = new TestOutputHelperLoggerMock<RabbitMQMessageBus>(_testOutputHelper);
        var serviceProvider = new Mock<IServiceProvider>();
        var rabbitMqOptions = Options.Create(rabbitMqSettings);
        _messageBus = new RabbitMQMessageBus(logger, serviceProvider.Object, rabbitMqOptions, isDurable: true, rabbitMqOptions.Value.RawMessageQueueName,
            needDeadMessageQueue: true, deliveryLimit: rabbitMqSettings.DeliveryLimit, queueMaxLength: rabbitMqSettings.TrackerQueueMaxLength);
        serviceProvider.Setup(s => s.GetService(typeof(TestTcpTrackerServerMessageHandler))).Returns(new TestTcpTrackerServerMessageHandler(_messageBus, receivedMessages));

        string? imageTag = Environment.GetEnvironmentVariable("DOCKER_TAG");
        if (imageTag is null)
        {
            imageTag = "latest";
        }
        _testOutputHelper.WriteLine($"imageTag = {imageTag}");

        byte[] appSettings = GetAppsettingsFile(containerConfigName);

        var builder = new ContainerBuilder()
                    .WithImage($"fms-tracker-tcp-server:{imageTag}")
                    .WithResourceMapping(appSettings, "/app/config/appsettings.json")
                    .WithPortBinding(6000, true)
                    // Wait until the HTTP endpoint of the _trackerMessageHandlerContainer is available.
                    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6000));
        if (ciEnvironment is null)
        {
            builder = builder.WithNetwork("integration-tests");
        }
        // Build the _trackerMessageHandlerContainer configuration.
        _container = builder.Build();
    }

    public async Task InitializeAsync()
    {
        try
        {
            await _container.StartAsync();
        }
        catch
        {
            var logs = await _container.GetLogsAsync();
            _testOutputHelper.WriteLine("Container failed to start - stdout:");
            _testOutputHelper.WriteLine(logs.Stdout);

            _testOutputHelper.WriteLine("Container failed to start - stderr:");
            _testOutputHelper.WriteLine(logs.Stderr);
            throw;
        }
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [Fact]
    public async void TrackerTcpServer_ShouldReceiveMessage()
    {
        // очистим очереди, принимающие участие в тесте
        _messageBus.PurgeQueue();

        // подпишемся на сообщения из очереди TcpServer, получающего сообщение
        // это позволит проверить, приходят ли к нему сообщения
        _messageBus.Subscribe<TestTcpTrackerServerMessageHandler>();

        // Создаём ТcpClient и отправляем от него тестовое сообщение на TcpServer
        using var tcpClient = new TcpClient();
        try
        {
            var mappedPort = _container.GetMappedPublicPort(6000);
            await tcpClient.ConnectAsync(_container.Hostname, _container.GetMappedPublicPort(6000));
            _testOutputHelper.WriteLine(string.Format("Установлено соединение по адресу {0}:{1}",
                _container.Hostname, _container.GetMappedPublicPort(6000)));
        }
        catch (Exception e)
        {
            _testOutputHelper.WriteLine(string.Format("Ошибка при открытии соединения по адресу {0}:{1} - {2}",
                _container.Hostname, _container.GetMappedPublicPort(6000), e.Message));

            tcpClient.Dispose();
            throw;
        }
        NetworkStream stream = tcpClient.GetStream();
        CancellationToken stoppingToken = new CancellationToken();

        var response = await TestUtils.SendRequest(_testOutputHelper, "TestData/11-package.txt", stream, stoppingToken);

        // TcpServer должен принять сообщение, положить его в очередь и отправть ответ

        // дадим гарантированный запас времени на обработку    
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        // Выведем логи контейнеров, учавствовавших в тесте
        var logs = await _container.GetLogsAsync();
        _testOutputHelper.WriteLine("Container stdout:");
        _testOutputHelper.WriteLine(logs.Stdout);

        _testOutputHelper.WriteLine("Container stderr:");
        _testOutputHelper.WriteLine(logs.Stderr);

        // Проверим, что тест отработал корректно

        // Получен правильный ответ от TcpServer 
        response.Should().Be("2 4A 22");

        // TcpServer на который бало отправлено сообщение получил его и положил в очередь
        byte[] messageToCheck = TestUtils.ReadMessageFromFile("TestData/11-package.txt");
        receivedMessages.Count().Should().Be(1);
        var receivedRawTrackerMessage = GetTrackerRawMessage(receivedMessages.First());
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

    private byte[] GetAppsettingsFile(string settingsFileName)
    {
        var type = GetType();
        var name = @"configs.TrackerTcpServerTests." + settingsFileName;

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
        var name = @"configs.TrackerTcpServerTests." + settingsFileName;
        var rabbitMQOptions = new RabbitMQOptions();
        using (var configStream = type.Assembly.GetManifestResourceStream(type, name))
        {
            var config = new ConfigurationBuilder().AddJsonStream(configStream).Build();
            config.Bind("RabbitMQ", rabbitMQOptions);
        }
        return rabbitMQOptions;
    }
}

public class TestTcpTrackerServerMessageHandler : IBusMessageHandler
{
    private readonly IMessageBus _consumerBus;
    private readonly List<byte[]> _receivedMessages;

    public TestTcpTrackerServerMessageHandler(IMessageBus consumerBus,
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