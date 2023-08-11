namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class ViewBounds
{
    ///<summary>
    ///Широта левой-верхней точки области
    ///</summary>
    public double UpperLeftLatitude { get; set; }

    ///<summary>
    ///Долгота левой-верхней точки области
    ///</summary>
    public double UpperLeftLongitude { get; set; }

    ///<summary>
    ///Широта правой-нижней точки области
    ///</summary>
    public double BottomRightLatitude { get; set; }

    ///<summary>
    ///Долгота правой-нижней точки области
    ///</summary>
    public double BottomRightLongitude { get; set; }
}