namespace BioTonFMS.Infrastructure.MessageBus
{
    public class MessageDeliverEventArgs
    {
        /// <summary>
        /// The delivery tag for this delivery
        /// </summary>
        public ulong DeliveryTag { get; set; }
    }
}
