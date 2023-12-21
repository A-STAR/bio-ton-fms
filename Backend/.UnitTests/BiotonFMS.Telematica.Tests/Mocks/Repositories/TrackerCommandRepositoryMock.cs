using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.EF;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerCommands;
using BioTonFMS.Infrastructure.Persistence.Providers;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;

namespace BiotonFMS.Telematica.Tests.Mocks.Repositories;

public static class TrackerCommandRepositoryMock
{
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
    
    private static DateTime Now = SystemTime.UtcNow;

    public static List<TrackerCommand> SampleCommands => new()
    {
        new TrackerCommand
        {
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = Now
        },
        new TrackerCommand
        {
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = Now.AddSeconds(-10)
        },
        new TrackerCommand
        {
            Tracker = new Tracker{ExternalId = 2552},
            SentDateTime = Now.AddSeconds(-20)
        }
    };
}