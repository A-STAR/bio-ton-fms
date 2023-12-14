using System.Collections;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using BioTonFMS.Common.Testable;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;

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
    
    public static TrackerMessage[] Messages => new TrackerMessage[]
    {
        new()
        {
            Id = 1,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
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
            Id = 2,
            ExternalTrackerId = 2552,
            Imei = "123",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            Latitude = 42.432023,
            Longitude = 54.556861,
            SatNumber = 14,
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
            Id = 3,
            ExternalTrackerId = 1024,
            Imei = "512128256",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
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
            Id = 4,
            ExternalTrackerId = 1555,
            Imei = "6412825699",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(20),
            Latitude = 36.4323,
            Longitude = 28.55861,
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
            Id = 5,
            ExternalTrackerId = 128,
            Imei = "64128256",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(10),
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
        },
        new()
        {
            Id = 6,
            ExternalTrackerId = 89989,
            Imei = "12389989",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            Latitude = 49.432023,
            Longitude = 52.556861,
            SatNumber = 12,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 92.0,
            Direction = 2.8,
            FuelLevel = 81,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("F28AC4A2-5DD0-49DC-B1B5-3B161C39549A")
        },
        new()
        {
            Id = 7,
            ExternalTrackerId = 89989,
            Imei = "12389989",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            Latitude = 42.432023,
            Longitude = 54.556861,
            SatNumber = 14,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 90.0,
            Speed = 50.1,
            Direction = 2.7,
            FuelLevel = 80,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("829D3996-DB42-4777-A4D5-BB6D8A9E1B79")
        },
        new()
        {
            Id = 8,
            ExternalTrackerId = 89910,
            Imei = "123899897",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(40),
            Latitude = 49.432023,
            Longitude = 52.556861,
            SatNumber = 12,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 92.0,
            Direction = 2.8,
            FuelLevel = 81,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("C21AC4A2-5DA0-49DC-B1B5-3B161C30549A"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 14980,
                    TrackerTagId = TagsSeed.CanB0Id,
                    TagType = TagDataTypeEnum.Integer,
                    Id = 1,
                    TrackerMessageId = 9
                }
            }
        },
        new()
        {
            Id = 9,
            ExternalTrackerId = 89910,
            Imei = "123899897",
            ServerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            TrackerDateTime = SystemTime.UtcNow - TimeSpan.FromSeconds(30),
            Latitude = 42.432023,
            Longitude = 54.556861,
            SatNumber = 14,
            CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
            Altitude = 90.0,
            Speed = 50.1,
            Direction = 2.7,
            FuelLevel = 80,
            CoolantTemperature = 45,
            EngineSpeed = 901,
            PackageUID = Guid.Parse("819D3996-DB42-4777-A4D5-BB6D8A0E1B79"),
            Tags = new List<MessageTag>
            {
                new MessageTagInteger
                {
                    Value = 14981,
                    TrackerTagId = TagsSeed.CanB0Id,
                    TagType = TagDataTypeEnum.Integer,
                    Id = 1,
                    TrackerMessageId = 9
                }
            }
        }
    };
}