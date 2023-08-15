namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class MonitoringGeneralInfoDto
{
     /// <summary>
     /// Время в секундах, прошедшее с момента получения последнего сообщения
     /// </summary>
     public int? TimeSinceLastMessage {get;set;}
     
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
     /// Моточасы из последнего сообщения
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