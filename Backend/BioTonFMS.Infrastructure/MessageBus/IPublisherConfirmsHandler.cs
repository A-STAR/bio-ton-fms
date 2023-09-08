namespace BioTonFMS.Infrastructure.MessageBus;

public interface IPublisherConfirmsHandler
{
    void PublisherConfirm_Acked(ulong deliveryTag, bool multiple);

    void PublisherConfirm_Nacked(ulong deliveryTag, bool multiple);
}
