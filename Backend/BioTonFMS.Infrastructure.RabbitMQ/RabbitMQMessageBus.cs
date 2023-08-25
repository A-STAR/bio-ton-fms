using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Threading.Channels;

namespace BioTonFMS.Infrastructure.RabbitMQ
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly ILogger<RabbitMQMessageBus> _logger;
        private readonly IModel? _channel;

        private readonly List<Type> _handlers = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQOptions _rabbitMqSettings;
        private readonly string _queueName;
        private readonly bool _isDurable;
        private readonly int _deliveryLimit;

        public RabbitMQMessageBus(
            ILogger<RabbitMQMessageBus> logger,
            IServiceProvider serviceProvider,
            IOptions<RabbitMQOptions> rabbitMqOptions,
            bool isDurable,
            string queueName,
            bool needDeadMessageQueue = false,
            int deliveryLimit = 0
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqSettings = rabbitMqOptions.Value;
            _queueName = queueName;
            _isDurable = isDurable;
            _deliveryLimit = deliveryLimit;
            _logger.LogDebug("RabbitMQMessageBus параметры соединения - HostName = {HostName} Port = {Port} VirtualHost = {VirtualHost}, UserName = {UserName}, QueueName = {QueueName}, DeliveryLimit = {DeliveryLimit}",
                _rabbitMqSettings.Host, _rabbitMqSettings.Port, _rabbitMqSettings.VHost, _rabbitMqSettings.User, queueName, deliveryLimit);
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqSettings.Host,
                Port = _rabbitMqSettings.Port,
                VirtualHost = _rabbitMqSettings.VHost,
                Password = _rabbitMqSettings.Password,
                UserName = _rabbitMqSettings.User,
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            try
            {
                CreateQueue(isDurable, needDeadMessageQueue);
            }
            catch (OperationInterruptedException)
            {
                // Очередь уже существует, но с другими параметрами. Удаляем её и пересоздаём.
                _channel = connection.CreateModel();
                _channel.QueueDelete(queue: _queueName, ifUnused: false, ifEmpty: false);
                CreateQueue(isDurable, needDeadMessageQueue);
            }
        }

        private void CreateQueue(bool isDurable, bool needDeadMessageQueue)
        {
            IDictionary<string, object> args = null;
            if (needDeadMessageQueue)
            {
                args = new Dictionary<string, object>();
                args["x-dead-letter-exchange"] = "";
                args["x-dead-letter-routing-key"] = $"err_{_queueName}";
                args["x-overflow"] = "reject-publish";
                args["x-queue-type"] = "quorum";
                // Количество попыток передоставки
                args["x-delivery-limit"] = _deliveryLimit;
            }
            _channel.QueueDeclare(queue: _queueName, durable: isDurable, exclusive: false,
                                    autoDelete: false, arguments: args);
            if (needDeadMessageQueue)
            {
                _channel.QueueDeclare(queue: $"err_{_queueName}", durable: isDurable, exclusive: false,
                                        autoDelete: false, arguments: null);
            }
        }

        public void Publish(byte[] message)
        {
            IBasicProperties props = _channel!.CreateBasicProperties();
            props.Persistent = _isDurable;
            
            _channel.BasicPublish(exchange: "",
                                    routingKey: _queueName,
                                    basicProperties: props,
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
                queue: _queueName,
                autoAck: false,
                consumer: consumer
            );
        }

        public void Ack(ulong deliveryTag, bool multiple)
        {
            if (_channel != null)
            {
                _channel.BasicAck(deliveryTag, multiple);
            }
        }

        public void Nack(ulong deliveryTag, bool multiple, bool requeue)
        {
            if (_channel != null)
            {
                _channel.BasicNack(deliveryTag, multiple, requeue);
            }
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
            var messageDeliverEventArgs = new MessageDeliverEventArgs
            {
                DeliveryTag = eventArgs.DeliveryTag
            };
            try
            {
                await ProcessEvent(body, messageDeliverEventArgs);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка при обработке сообщения: {Body}", body);
            }
        }

        private async Task ProcessEvent(byte[] message, MessageDeliverEventArgs messageDeliverEventArgs)
        {
            _logger.LogTrace("Обработка сообщения RabbitMQ");

            List<Type> subscriptions = _handlers;
            foreach (var subscription in subscriptions)
            {
                var handler = _serviceProvider.GetService(subscription);
                if (handler == null)
                {
                    _logger.LogWarning($"Нет зарегистрированных обработчиков для подписки на сообщение типа {subscription.Name}");
                    continue;
                }

                var eventHandler = (IBusMessageHandler)handler;
                await Task.Yield();
                await eventHandler.HandleAsync(message, messageDeliverEventArgs);
            }
            _logger.LogTrace("Сообщение обработано");
        }
    }
}