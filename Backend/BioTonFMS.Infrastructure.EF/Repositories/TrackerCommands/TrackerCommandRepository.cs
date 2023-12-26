using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Paging.Extensions;
using BioTonFMS.Infrastructure.Persistence;
using BioTonFMS.Infrastructure.Persistence.Providers;
using Microsoft.EntityFrameworkCore;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;

public class TrackerCommandRepository : Repository<TrackerCommand, BioTonDBContext>, ITrackerCommandRepository
{
    public TrackerCommandRepository(IKeyValueProvider<TrackerCommand, int> keyValueProvider,
        IQueryableProvider<TrackerCommand> queryableProvider,
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactory)
        : base(keyValueProvider, queryableProvider, unitOfWorkFactory)
    {
    }

    public TrackerCommand? GetWithoutCaching(int key)
    {
        TrackerCommand? trackerCommand = QueryableProvider.Linq()
            .AsNoTracking().SingleOrDefault(t => t.Id == key);
        return trackerCommand;
    }

    /// <summary>
    /// ¬озвращает статистику по командам трекера дл€ просмотра сообщений дл€ зададанного трекера и временного диапазона
    /// </summary>
    public ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();
        IQueryable<TrackerCommand> commands = QueryableProvider.Linq()
            .AsNoTracking()
            .Where(x => x.Tracker != null &&
                        x.Tracker.ExternalId == externalId &&
                        x.SentDateTime >= startUtc &&
                        x.SentDateTime <= endUtc);

        return new ViewMessageStatisticsDto
        {
            NumberOfMessages = commands.Count(),
            TotalTime = (commands.Select(x => x.SentDateTime).DefaultIfEmpty().Max() -
                         commands.Select(x => x.SentDateTime).DefaultIfEmpty().Min()).Seconds
        };
    }

    /// <summary>
    /// ѕостранично возвращает массив событий с информацией о командах отправленных на трекер дл€ зададанного трекера и временного диапазона
    /// </summary>
    public PagedResult<CommandMessageDto> GetCommandMessages(int externalId, DateTime start, DateTime end, int pageNum, int pageSize)
    {
        var startUtc = start.ToUniversalTime();
        var endUtc = end.ToUniversalTime();

        PagedResult<TrackerCommand> commands = QueryableProvider.Linq()
            .AsNoTracking()
            .Where(x => x.Tracker != null &&
                    x.Tracker.ExternalId == externalId &&
                    x.SentDateTime >= startUtc &&
                    x.SentDateTime <= endUtc)
            .OrderBy(x => x.SentDateTime)
            .AsNoTracking()
            .GetPagedQueryable(pageNum, pageSize);

        return new PagedResult<CommandMessageDto>
        {
            Results = commands.Results.Select((cmd, idx) => new CommandMessageDto
            {
                Id = cmd.Id,
                Num = (commands.CurrentPage - 1) * commands.PageSize + idx + 1,
                CommandDateTime = cmd.SentDateTime,
                CommandText = cmd.CommandText,
                ExecutionTime = cmd.ResponseDateTime,
                CommandResponseText = cmd.ResponseText
            }).ToList(),
            PageSize = commands.PageSize,
            CurrentPage = commands.CurrentPage,
            TolalRowCount = commands.TolalRowCount,
            TotalPageCount = commands.TotalPageCount,
        };
    }

    /// <summary>
    /// ”дал€ет команды из списка
    /// </summary>
    /// <param name="messageIds">список идентификаторов дл€ удалени€</param>
    public void DeleteCommands(int[] commandIds)
    {
        var existingFromList = QueryableProvider.Linq().Where(cmd => commandIds.Contains(cmd.Id)).ToList();
        if (commandIds.Length > existingFromList.Count())
        {
            throw new ArgumentException($"ƒл€ некоторых идентификаторов из списка не найдены сообщени€");
        }

        foreach (var message in existingFromList)
        {
            base.Remove(message);
        }
    }
}