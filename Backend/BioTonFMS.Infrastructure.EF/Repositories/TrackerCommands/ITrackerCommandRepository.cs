using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;

public interface ITrackerCommandRepository : IRepository<TrackerCommand>
{
    TrackerCommand? GetWithoutCaching(int key);

    /// <summary>
    /// ¬озвращает статистику по командам трекера дл€ просмотра сообщений дл€ зададанного трекера и временного диапазона
    /// </summary>
    ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end);

    /// <summary>
    /// ѕостранично возвращает массив с синформацией о командах отправленных на трекер дл€ зададанного трекера и временного диапазона
    /// </summary>
    PagedResult<CommandMessageDto> GetCommandMessages(int externalId, DateTime start, DateTime end, int pageNum, int pageSize);

    /// <summary>
    /// ѕостранично возвращает массив с синформацией о командах отправленных на трекер дл€ зададанного трекера и даты
    /// </summary>
    IList<TrackerCommand> GetCommandsForDate(int[] trackerIds, DateOnly date, bool forUpdate);

    /// <summary>
    /// ”дал€ет сообщени€ из списка
    /// </summary>
    /// <param name="messageIds">список идентификаторов дл€ удалени€</param>
    void DeleteCommands(int[] commandIds);

    
}