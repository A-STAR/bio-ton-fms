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
}