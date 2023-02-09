using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BioTonFMS.Infrastructure.RabbitMQ
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly ILogger<RabbitMQMessageBus> _logger;
        private readonly IModel? _channel;
        private readonly MessageBrokerSettingsOptions _settings;

        private readonly List<Type> _handlers = new();
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQMessageBus(
            ILogger<RabbitMQMessageBus> logger,
            IServiceProvider serviceProvider,
            IOptions<MessageBrokerSettingsOptions> opts
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _settings = opts.Value;
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                Password = _settings.Password,
                UserName = _settings.UserName,
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: _settings.Queue,
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);
        }

        public void Publish(byte[] message)
        {

            _channel.BasicPublish(exchange: "",
                                    routingKey: _settings.Queue,
                                    basicProperties: null,
                                    body: message);
        }

        public void Subscribe<TBusMessageHandler>()
            where TBusMessageHandler : IBusMessageHandler
        {
            AddSubscription<TBusMessageHandler>();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += Consumer_Received;

            _channel.BasicConsume
            (
                queue: _settings.Queue,
                autoAck: true,
                consumer: consumer
            );
        }

        private void AddSubscription<TBusMessageHandler>() where TBusMessageHandler : IBusMessageHandler
        {
            Type handlerType = typeof(TBusMessageHandler);
            if (_handlers.Any(h => h == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered ", nameof(handlerType));
            }
            _handlers.Add(handlerType);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            byte[] body = eventArgs.Body.ToArray();
            try
            {
                await ProcessEvent(body);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при обработке сообщения: {Body}", body);
            }
        }

        private async Task ProcessEvent(byte[] message)
        {
            _logger.LogTrace("Обработка сообщения RabbitMQ");

            var subscriptions = _handlers;
            foreach (var subscription in subscriptions)
            {
                var handler = _serviceProvider.GetService(subscription);
                if (handler == null)
                {
                    _logger.LogWarning("Нет зарегистрированных обработчиков");
                    continue;
                }

                var eventHandler = (IBusMessageHandler)handler;
                await Task.Yield();
                await eventHandler.HandleAsync(message);
            }
            _logger.LogTrace("Сообщение обработано");
        }
    }
}