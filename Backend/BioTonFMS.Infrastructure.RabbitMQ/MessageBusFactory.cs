using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioTonFMS.Infrastructure.RabbitMQ;

public class MessageBusFactory
{
    static Dictionary<MessgingBusType, IMessageBus> _busList = new();

    public static IMessageBus CreateOrGetBus(MessgingBusType busType, IServiceProvider serviceProvider, Func<IServiceProvider, RabbitMQOptions, IMessageBus>? rawMessageBusFunc = null)
    {
        if (_busList.ContainsKey(busType))
        {
            return _busList[busType];
        }
        else
        {
            var rabbitLogger = serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>();
            var rabbitOptions = serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>();
            IMessageBus bus = busType switch
            {
                MessgingBusType.Retranslation => new RabbitMQMessageBus(
                    rabbitLogger, serviceProvider, rabbitOptions,
                    isDurable: true, "Retranslation", needDeadMessageQueue: true, deliveryLimit: rabbitOptions.Value.DeliveryLimit),
                MessgingBusType.Consuming => new RabbitMQMessageBus(
                    rabbitLogger, serviceProvider, rabbitOptions,
                    isDurable: true, rabbitOptions.Value.RawMessageQueueName, needDeadMessageQueue: true,
                    deliveryLimit: rabbitOptions.Value.DeliveryLimit, queueMaxLength: rabbitOptions.Value.TrackerQueueMaxLength),
                MessgingBusType.TrackerCommandsReceive => new RabbitMQMessageBus(
                    rabbitLogger, serviceProvider, rabbitOptions,
                    isDurable: true, "tracker-command-receive", needDeadMessageQueue: true),
                MessgingBusType.TrackerCommandsSend => new RabbitMQMessageBus(
                    rabbitLogger, serviceProvider, rabbitOptions,
                    isDurable: true, "tracker-command-send", needDeadMessageQueue: true),
                MessgingBusType.RawTrackerMessages => GetRawTrackerBus(rawMessageBusFunc, serviceProvider, rabbitOptions.Value),

                _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
            };
            _busList.Add(busType, bus);
            return bus;
        }
    }

    static private IMessageBus GetRawTrackerBus(Func<IServiceProvider, RabbitMQOptions, IMessageBus>? rawMessageBusFunc, IServiceProvider serviceProvider, RabbitMQOptions rabbitOptions)
    {
        if (rawMessageBusFunc == null)
        {
            throw new NotImplementedException();
        }
        else
        {
            return rawMessageBusFunc(serviceProvider, rabbitOptions);
        }
    }
}
