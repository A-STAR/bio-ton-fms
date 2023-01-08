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
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel? _channel;

        private List<Type> _handlers = new List<Type>();
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQMessageBus(
            ILogger<RabbitMQMessageBus> logger,
            IServiceProvider serviceProvider,
            IOptions<MessageBrokerSettingsOptions> opts
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _factory = new ConnectionFactory() { HostName = opts.Value.HostName };
            _factory.AutomaticRecoveryEnabled = true;
            _factory.DispatchConsumersAsync = true;

            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "RawTrackerMessages",
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);
        }

        public void Publish(byte[] message)
        {

            _channel.BasicPublish(exchange: "",
                                    routingKey: "RawTrackerMessages",
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
                queue: "RawTrackerMessages",
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
                _logger.LogWarning(ex, "Ошибка при обработке сообщения: {body}.", body);
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