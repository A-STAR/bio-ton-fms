using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;

public interface ITrackerCommandRepository : IRepository<TrackerCommand>
{
    TrackerCommand? GetWithoutCaching(int key);

    /// <summary>
    /// Возвращает статистику по командам трекера для просмотра сообщений для зададанного трекера и временного диапазона
    /// </summary>
    ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end);

    /// <summary>
    /// Постранично возвращает массив событий с синформацией о командах отправленных на трекер для зададанного трекера и временного диапазона
    /// </summary>
    PagedResult<CommandMessageDto> GetCommandMessages(int externalId, DateTime start, DateTime end, int pageNum, int pageSize);
}