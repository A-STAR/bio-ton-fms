using BioTonFMS.Domain;
using BioTonFMS.Domain.MessageStatistics;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BioTonFMS.Infrastructure.Persistence.Specifications;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using LinqSpecs;

namespace BiotonFMS.Telematica.Tests.Mocks.Repositories;

public class TrackerCommandRepositoryMock : ITrackerCommandRepository
{
    private int _idSeq;
    private readonly List<TrackerCommand> _mockList;

    public TrackerCommandRepositoryMock()
    {
        _mockList = new List<TrackerCommand>();
        _idSeq = 0;
    }

    public TrackerCommand? this[int key] => _mockList.FirstOrDefault(x => x.Id == key);

    public TrackerCommand? GetWithoutCaching(int key)
    {
        return _mockList.FirstOrDefault(x => x.Id == key);
    }

    public ViewMessageStatisticsDto GetStatistics(int externalId, DateTime start, DateTime end)
    {
        throw new NotImplementedException();
    }

    public int Count => _mockList.Count;

    public void Remove(TrackerCommand entity)
    {
        _mockList.Remove(entity);
    }

    public void Add(TrackerCommand entity)
    {
        _idSeq++;
        entity.Id = _idSeq;
        _mockList.Add(entity);
    }

    public void Update(TrackerCommand entity)
    {
        throw new NotImplementedException();
    }

    public IBeginSpecification<TrackerCommand> BeginSpecification(Specification<TrackerCommand> specification)
    {
        throw new NotImplementedException();
    }
    
    public static ITrackerCommandRepository GetStub(ICollection<TrackerCommand>? commands = null)
    {
        commands ??= SampleCommands;
        
        IKeyValueProvider<TrackerCommand, int> keyValueProviderMock =
            new KeyValueProviderMock<TrackerCommand, int>(commands);
        IQueryableProvider<TrackerCommand> commandQueryProviderMock =
            new QueryableProviderMock<TrackerCommand>(commands);
        UnitOfWorkFactory<BioTonDBContext> unitOfWorkFactoryMock =
            new BioTonDBContextUnitOfWorkFactoryMock();

        return new TrackerCommandRepository(keyValueProviderMock, commandQueryProviderMock, unitOfWorkFactoryMock);
    }
    
    public static List<TrackerCommand> SampleCommands => new()
    {
        new TrackerCommand
        {
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = DateTime.UnixEpoch
        },
        new TrackerCommand
        {
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = DateTime.UnixEpoch.AddSeconds(-10)
        },
        new TrackerCommand
        {
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = DateTime.UnixEpoch.AddSeconds(-20)
        }
    };
}