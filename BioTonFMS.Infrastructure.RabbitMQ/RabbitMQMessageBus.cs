using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BioTonFMS.Infrastructure.RabbitMQ
{
    public class RabbitMQMessageBus : IMessageBus
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel? _channel;

        public RabbitMQMessageBus(IOptions<MessageBrockerSettingsOptions> opts)
        {
            _factory = new ConnectionFactory() { HostName = opts.Value.HostName };
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

        public void Subscribe<TEvenArgs>(EventHandler<TEvenArgs> handler)
        {
            var consumer = new EventingBasicConsumer(_channel);
            EventHandler<BasicDeliverEventArgs>? rabitHandler = handler as EventHandler<BasicDeliverEventArgs>;
            if (rabitHandler != null)
            {
                consumer.Received += rabitHandler;
                _channel.BasicConsume(queue: "RawTrackerMessages",
                                        autoAck: true,
                                        consumer: consumer);
            }
        }
    }
}