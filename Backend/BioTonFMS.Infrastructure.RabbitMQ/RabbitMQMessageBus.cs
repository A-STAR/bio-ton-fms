using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace BioTonFMS.Infrastructure.RabbitMQ
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly ILogger<RabbitMQMessageBus> _logger;

        private readonly List<Type> _handlers = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQOptions _rabbitMqSettings;
        private readonly string _queueName;
        private readonly bool _isDurable;
        private readonly int _deliveryLimit;
        private readonly bool _needDeadMessageQueue;
        private readonly int? _queueMaxLength;
        private bool _initialized = false;
        private IModel? _channel;
        private IPublisherConfirmsHandler? _confirmHandler = null;

        public RabbitMQMessageBus(
            ILogger<RabbitMQMessageBus> logger,
            IServiceProvider serviceProvider,
            IOptions<RabbitMQOptions> rabbitMqOptions,
            bool isDurable,
            string queueName,
            bool needDeadMessageQueue = false,
            int deliveryLimit = 0,
            int? queueMaxLength = null
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqSettings = rabbitMqOptions.Value;
            _queueName = queueName;
            _isDurable = isDurable;
            _deliveryLimit = deliveryLimit;
            _needDeadMessageQueue = needDeadMessageQueue;
            _queueMaxLength = queueMaxLength;
            _logger.LogDebug("RabbitMQMessageBus параметры соединения - HostName = {HostName} Port = {Port} VirtualHost = {VirtualHost}, UserName = {UserName}, QueueName = {QueueName}, DeliveryLimit = {DeliveryLimit}, MaxLength = {MaxLength}",
                _rabbitMqSettings.Host, _rabbitMqSettings.Port, _rabbitMqSettings.VHost, _rabbitMqSettings.User, queueName, deliveryLimit, _queueMaxLength);
        }

        public void SetPublisherConfirmsHandler(IPublisherConfirmsHandler confirmHandler)
        {
            _confirmHandler = confirmHandler;
        }

        public ulong Publish(byte[] message)
        {
            CheckIfInitialized();
            IBasicProperties props = _channel!.CreateBasicProperties();
            props.Persistent = _isDurable;
            var nextSeqNumber = _channel.NextPublishSeqNo;
            _channel.BasicPublish(exchange: "",
                                    routingKey: _queueName,
                                    basicProperties: props,
                                    body: message);
            return nextSeqNumber;
        }

        public void Subscribe<TBusMessageHandler>()
            where TBusMessageHandler : IBusMessageHandler
        {
            CheckIfInitialized();
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
            CheckIfInitialized();
            if (_channel != null)
            {
                _channel.BasicAck(deliveryTag, multiple);
            }
        }

        public void Nack(ulong deliveryTag, bool multiple, bool requeue)
        {
            CheckIfInitialized();
            if (_channel != null)
            {
                _channel.BasicNack(deliveryTag, multiple, requeue);
            }
        }

        public void PurgeQueue()
        {
            CheckIfInitialized();
            _channel?.QueuePurge(_queueName);
        }

        public uint? GetMessageCount()
        {
            CheckIfInitialized();
            return _channel?.MessageCount(_queueName);
        }

        private void CheckIfInitialized()
        {
            if (!_initialized)
            {
                InitializeChannel();
            }
        }

        private void InitializeChannel()
        {
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
            _channel = CreateChannel(connection);
            try
            {
                CreateQueue(_channel, _isDurable, _needDeadMessageQueue);
            }
            catch (OperationInterruptedException ex)
            {
                _logger.LogDebug("CreateQueue exception {Message}", ex.Message);
                // Очередь уже существует, но с другими параметрами. Удаляем её и пересоздаём.
                _channel = CreateChannel(connection);
                _channel.QueueDelete(queue: _queueName, ifUnused: false, ifEmpty: false);
                CreateQueue(_channel, _isDurable, _needDeadMessageQueue);
            }

            _initialized = true;
        }

        private IModel CreateChannel(IConnection connection)
        {
            IModel channel = connection.CreateModel();
            if (_confirmHandler != null)
            {
                channel.ConfirmSelect();
                channel.BasicAcks += PublisherConfirm_Acked;
                channel.BasicNacks += PublisherConfirm_Nacked;
            }
            return channel;
        }

        private void PublisherConfirm_Acked(object? sender, BasicAckEventArgs eventArgs)
        {
            if (_confirmHandler != null)
            {
                _confirmHandler.PublisherConfirm_Acked(eventArgs.DeliveryTag, eventArgs.Multiple);
            }
        }

        private void PublisherConfirm_Nacked(object? sender, BasicNackEventArgs eventArgs)
        {
            if (_confirmHandler != null)
            {
                _confirmHandler.PublisherConfirm_Nacked(eventArgs.DeliveryTag, eventArgs.Multiple);
            }
        }

        private void CreateQueue(IModel channel, bool isDurable, bool needDeadMessageQueue)
        {
            IDictionary<string, object> args = null;
            if (needDeadMessageQueue)
            {
                args = new Dictionary<string, object>();
                args["x-dead-letter-exchange"] = "";
                args["x-dead-letter-routing-key"] = $"err_{_queueName}";
                if (_queueMaxLength is not null)
                {
                    args["x-max-length"] = _queueMaxLength;
                }
                args["x-overflow"] = "reject-publish";
                args["x-queue-type"] = "quorum";
                // Количество попыток передоставки
                args["x-delivery-limit"] = _deliveryLimit;
            }
            _logger.LogDebug("About to QueueDeclare {QueueName} {IsDurable} {Args}", _queueName, isDurable, args);
            channel.QueueDeclare(queue: _queueName, durable: isDurable, exclusive: false,
                                    autoDelete: false, arguments: args);
            if (needDeadMessageQueue)
            {
                _logger.LogDebug("About to DeadMessageQueue Declare {QueueName} {IsDurable}", _queueName, isDurable);
                channel.QueueDeclare(queue: $"err_{_queueName}", durable: isDurable, exclusive: false,
                                        autoDelete: false, arguments: null);
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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var handler = scope.ServiceProvider.GetService(subscription);
                    if (handler == null)
                    {
                        _logger.LogWarning($"Нет зарегистрированных обработчиков для подписки на сообщение типа {subscription.Name}");
                        continue;
                    }

                    var eventHandler = (IBusMessageHandler)handler;
                    await Task.Yield();
                    await eventHandler.HandleAsync(message, messageDeliverEventArgs.DeliveryTag);
                }
            }
            _logger.LogTrace("Сообщение обработано");
        }
    }
}