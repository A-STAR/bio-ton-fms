using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Domain.Monitoring;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.Models.Filters;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;

public interface ITrackerMessageRepository : IRepository<TrackerMessage>
{
    TrackerMessage? this[long key] { get; }
    bool ExistsByUid(Guid uid);

    IList<TrackerMessage> GetMessages(bool forUpdate = false);

    IList<TrackerMessage> GetMessagesForTrackers(int[] trackerExternalIds, bool forUpdate = false);

    IList<TrackerMessage> GetTrackerMessagesForDate(int[] trackerExternalIds, DateOnly date, bool forUpdate = false);

    TrackerStandardParameters GetStandardParameters(int externalId);

    IList<TrackerParameter> GetParameters(int externalId);

    TrackerMessage? GetLastMessageFor(int externalId);

    PagedResult<ParametersHistoryRecord> GetParametersHistory(ParametersHistoryFilter filter);

    IDictionary<int, VehicleStatus> GetVehicleStates(int[] externalIds, int trackerAddressValidMinutes);

    IDictionary<int, TrackPointInfo[]> GetTracks(DateTime trackStartTime, DateTime trackEndTime,
        params int[] externalIds);

    /// <summary>
    /// Информация о местонахождении трекеров
    /// </summary>
    /// <param name="externalIds">Внешние id трекеров</param>
    /// <returns>Локации трекеров в формате Longitude, Latitude</returns>
    IDictionary<int, (double Lat, double Long)> GetLocations(params int[] externalIds);

    /// <summary>
    ///  Получить статистику для трекера за период
    /// </summary>
    /// <param name="externalId">Внешний идентификатор трекера</param>
    /// <param name="start">Начало периода</param>
    /// <param name="end">Конец периода</param>
    /// <returns></returns>
    ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end);

    /// <summary>
    /// Возвращает массив событий с данными от трекера для зададанного трекера и временного диапазода  
    /// </summary>
    PagedResult<TrackerDataMessageDto> GetTrackertDataMessages(int externalId, DateTime start, DateTime end, int pageNum, int pageSize);

    /// <summary>
    /// Удаляет сообщения из списка
    /// </summary>
    /// <param name="messageIds">список идентификаторов для удаления</param>
    void DeleteMessages(long[] messageIds);
}