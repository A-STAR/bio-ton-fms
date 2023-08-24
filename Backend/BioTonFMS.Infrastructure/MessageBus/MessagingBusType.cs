namespace BioTonFMS.Infrastructure.MessageBus
{
    public enum MessgingBusType
    {
        Consuming,
        Retranslation,
        RawTrackerMessages,
        TrackerCommandsSend,
        TrackerCommandsReceive,
    }
}
