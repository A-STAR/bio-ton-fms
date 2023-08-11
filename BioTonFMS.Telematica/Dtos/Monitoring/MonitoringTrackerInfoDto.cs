using BioTonFMS.Domain.TrackerMessages;

namespace BioTonFMS.Telematica.Dtos.Monitoring;

public class MonitoringTrackerInfoDto
{
     public string TrackerType {get;set;}
     
     /// <summary>
     /// Внешний ID трекера
     /// </summary>
     public int ExternalId {get;set;}
     
     /// <summary>
     /// IMEI трекера
     /// </summary>
     public string Imei {get;set;}
     
     /// <summary>
     /// Номер сим карты
     /// </summary>
     public string SimNumber {get;set;}
     
     /// <summary>
     /// Параметры трекера
     /// </summary>
     public TrackerParameter[] Parameters {get;set;}
     
     /// <summary>
     /// Список датчиков и значения датчиков с соответствующими единицами измерения,
     /// которые рассчитаны по последнему пришедшему сообщению у которых признак “видимость” = TRUE.
     /// </summary>
     public TrackerSensorDto[] Sensors {get;set;}
}