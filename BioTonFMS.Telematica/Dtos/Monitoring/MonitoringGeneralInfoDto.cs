namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class MonitoringGeneralInfoDto
{
     /// <summary>
     /// Дата и время последнего сообщения
     /// </summary>
     public DateTime? LastMessageTime {get;set;}
     
     /// <summary>
     /// Скорость из последнего сообщения
     /// </summary>
     public double? Speed {get;set;}
     
     /// <summary>
     /// Пробег из последнего сообщения (значение тега can_b0 умноженное на 5)
     /// </summary>
     public int? Mileage {get;set;}
     
     /// <summary>
     /// Моточасы из последнего сообщения (значение Can32BitR0Id поделённое на 100)
     /// </summary>
     public int? EngineHours {get;set;}
     
     /// <summary>
     /// Количество спутников из последнего сообщения
     /// </summary>
     public int? SatellitesNumber {get;set;}
     
     /// <summary>
     /// Широта из последнего сообщения
     /// </summary>
     public double? Latitude {get;set;}
     
     /// <summary>
     /// Долгота из последнего сообщения
     /// </summary>
     public double? Longitude {get;set;}
}
