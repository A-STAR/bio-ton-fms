namespace BioTonFMS.Telematica.Dtos.MessagesView;

public class MessagesViewTrackRequest
{
    public int VehicleId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}