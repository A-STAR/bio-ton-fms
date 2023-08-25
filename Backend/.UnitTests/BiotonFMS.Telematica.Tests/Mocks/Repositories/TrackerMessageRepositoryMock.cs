using System.Collections;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;

namespace BiotonFMS.Telematica.Tests.Mocks.Repositories;

public class TrackerMessageRepositoryMock
{
    public static ITrackerMessageRepository GetStub(ICollection<TrackerMessage>? messages = null)
    {
        messages ??= Messages;
        
        var kvp = new KeyValueProviderMock<TrackerMessage, int>(messages);
        var qp = new QueryableProviderMock<TrackerMessage>(messages);
        var uow = new MessagesDBContextUnitOfWorkFactoryMock();
        var repo = new TrackerMessageRepository(kvp, qp, uow, TrackerTagRepositoryMock.GetStub());
        
        return repo;
    }
    
    private static TrackerMessage[] Messages => new TrackerMessage[]
    {
        new()
        {
            Id = 0,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = DateTime.UtcNow - TimeSpan.FromSeconds(4),
            TrackerDateTime = DateTime.UtcNow - TimeSpan.FromSeconds(4),
            Latitude = 49.432023,
            Longitude = 52.556861,
            SatNumber = 12,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 97.0,
            Direction = 2.8,
            FuelLevel = 100,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("F28AC4A2-5DD0-49DC-B8B5-3B161C39546A"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 1234,
                    TrackerTagId = 5,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagByte
                {
                    Value = 6,
                    TrackerTagId = 10,
                    TagType = TagDataTypeEnum.Byte
                },
                new MessageTagBits
                {
                    Value = new BitArray(new byte[] { 215 }),
                    TrackerTagId = 15,
                    TagType = TagDataTypeEnum.Bits
                }
            }
        },
        new()
        {
            Id = 1,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = DateTime.UtcNow - TimeSpan.FromSeconds(2),
            TrackerDateTime = DateTime.UtcNow - TimeSpan.FromSeconds(2),
            Latitude = 49.432023,
            Longitude = 52.556861,
            SatNumber = 12,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 97.0,
            Speed = 12.1,
            Direction = 2.8,
            FuelLevel = 100,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("829C3996-DB42-4777-A4D5-BB6D8A9E3B79"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 12345,
                    TrackerTagId = 5,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagByte
                {
                    Value = 6,
                    TrackerTagId = 10,
                    TagType = TagDataTypeEnum.Byte
                },
                new MessageTagInteger
                {
                    Value = 2134,
                    TrackerTagId = 24,
                    TagType = TagDataTypeEnum.Integer
                }
            }
        },
        new()
        {
            Id = 2,
            ExternalTrackerId = 1024,
            Imei = "512128256",
            ServerDateTime = DateTime.UtcNow - TimeSpan.FromSeconds(1),
            TrackerDateTime = DateTime.UtcNow - TimeSpan.FromSeconds(1),
            Latitude = 39.4323,
            Longitude = 12.55861,
            SatNumber = 1,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 92.0,
            Speed = null,
            Direction = 2.1,
            FuelLevel = 90,
            CoolantTemperature = 40,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("719C3996-DB32-4777-A4F5-BC0D8A9E3B96"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 12345,
                    TrackerTagId = 5,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagByte
                {
                    Value = 6,
                    TrackerTagId = 10,
                    TagType = TagDataTypeEnum.Byte
                },
                new MessageTagInteger
                {
                    Value = 2134,
                    TrackerTagId = 24,
                    TagType = TagDataTypeEnum.Integer
                }
            }
        },
        new()
        {
            Id = 3,
            ExternalTrackerId = 128,
            Imei = "64128256",
            ServerDateTime = DateTime.UtcNow - TimeSpan.FromSeconds(1),
            Latitude = 39.4323,
            Longitude = 12.55861,
            SatNumber = 19,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGsm,
            Altitude = 92.0,
            Speed = 0,
            Direction = 2.1,
            FuelLevel = 90,
            CoolantTemperature = 40,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("719C3996-DB32-4777-A4F5-BC0D8A9E3B96"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 12345,
                    TrackerTagId = 5,
                    TagType = TagDataTypeEnum.Integer
                },
                new MessageTagByte
                {
                    Value = 6,
                    TrackerTagId = 10,
                    TagType = TagDataTypeEnum.Byte
                },
                new MessageTagInteger
                {
                    Value = 2134,
                    TrackerTagId = 24,
                    TagType = TagDataTypeEnum.Integer
                }
            }
        }
    };
}