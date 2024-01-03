using BioTonFMS.Domain;
using BioTonFMS.Domain.MessagesView;
using BioTonFMS.Infrastructure.Paging;
using BioTonFMS.Infrastructure.Persistence;

namespace BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;

public interface ITrackerCommandRepository : IRepository<TrackerCommand>
{
    TrackerCommand? GetWithoutCaching(int key);

    /// <summary>
    /// ���������� ���������� �� �������� ������� ��� ��������� ��������� ��� ����������� ������� � ���������� ���������
    /// </summary>
    ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end);

    /// <summary>
    /// ����������� ���������� ������ � ������������ � �������� ������������ �� ������ ��� ����������� ������� � ���������� ���������
    /// </summary>
    PagedResult<CommandMessageDto> GetCommandMessages(int externalId, DateTime start, DateTime end, int pageNum, int pageSize);

    /// <summary>
    /// ����������� ���������� ������ � ������������ � �������� ������������ �� ������ ��� ����������� ������� � ����
    /// </summary>
    IList<TrackerCommand> GetCommandsForDate(int[] trackerIds, DateOnly date, bool forUpdate);

    /// <summary>
    /// ������� ��������� �� ������
    /// </summary>
    /// <param name="messageIds">������ ��������������� ��� ��������</param>
    void DeleteCommands(int[] commandIds);

    
}