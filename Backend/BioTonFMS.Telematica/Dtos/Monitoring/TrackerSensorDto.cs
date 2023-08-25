namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class TrackerSensorDto
{
     /// <summary>
     /// Название датчика
     /// </summary>
     public string Name {get;set;}
     
     /// <summary>
     /// Значение датчика рассчитанное по последнему сообщению и преобразованное в строку
     /// </summary>
     public string? Value {get;set;}
     
     /// <summary>
     /// Название единицы измерения датчика
     /// </summary>
     public string Unit {get;set;}
}