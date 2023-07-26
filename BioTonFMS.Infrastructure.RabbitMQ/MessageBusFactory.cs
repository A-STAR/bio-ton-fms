using BioTonFMS.Common.Settings;
using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioTonFMS.Infrastructure.RabbitMQ;

public class MessageBusFactory
{
    static Dictionary<MessgingBusType, IMessageBus> _busList = new();

    public static IMessageBus CreateOrGetBus(MessgingBusType busType, IServiceProvider serviceProvider, Func<IServiceProvider, IMessageBus> rawMessageBusFunc = null)
    {
        if (_busList.ContainsKey(busType))
        {
            return _busList[busType];
        }
        else
        {
            IMessageBus bus = busType switch
            {
                MessgingBusType.Retranslation => new RabbitMQMessageBus(
                    serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
                    serviceProvider,
                    serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>(),
                    "Retranslation"),
                MessgingBusType.Consuming => new RabbitMQMessageBus(
                    serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
                    serviceProvider,
                    serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>(),
                    "RawTrackerMessages-primary"),
                MessgingBusType.TrackerCommandsReceive => new RabbitMQMessageBus(
                    serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
                    serviceProvider,
                    serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>(),
                    "tracker-command-receive"),
                MessgingBusType.TrackerCommandsSend => new RabbitMQMessageBus(
                    serviceProvider.GetRequiredService<ILogger<RabbitMQMessageBus>>(),
                    serviceProvider,
                    serviceProvider.GetRequiredService<IOptions<RabbitMQOptions>>(),
                    "tracker-command-send"),
                MessgingBusType.RawTrackerMessages => GetRawTrackerBus(rawMessageBusFunc, serviceProvider),

                _ => throw new ArgumentOutOfRangeException(nameof(busType), busType, null)
            };
            _busList.Add(busType, bus);
            return bus;
        }
    }

    static private IMessageBus GetRawTrackerBus(Func<IServiceProvider, IMessageBus> rawMessageBusFunc, IServiceProvider serviceProvider)
    {
        if (rawMessageBusFunc == null)
        {
            throw new NotImplementedException();
        }
        else
        {
            return rawMessageBusFunc(serviceProvider);
        }
    }
}
