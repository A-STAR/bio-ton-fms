using System.Collections;
using System.Globalization;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.ProtocolTags;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using BiotonFMS.Telematica.Tests.Mocks.Infrastructure;
using BioTonFMS.TrackerMessageHandler.MessageParsing;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Xunit.Abstractions;

namespace BiotonFMS.Telematica.Tests.GalileoskyTests;

public class GalileoskyMessageParserTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public GalileoskyMessageParserTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[]
            {
                "Расшифровка пакетов.docx",
                "01 1a 80 01 82 02 17 03 38 36 32 30 35 37 30 34 37 36 31 35 36 30 31 04 2c 07 20 c9", //48 0b 00
                new TrackerMessage[]
                {
                    new()
                    {
                        Imei = "862057047615601",
                        TrId = 1836,
                        Tags = new List<MessageTag>
                        {
                            new MessageTagInteger
                            {
                                Value = 130,
                                TrackerTagId = 1
                            },
                            new MessageTagInteger
                            {
                                Value = 23,
                                TrackerTagId = 2
                            }
                        }
                    }
                }
            },
            new object[]
            {
                "Расшифровка_пакет_а_JD_с_CAN_данными.docx",
                "01 B8 83 04 F8 09 10 05 47 20 48 50 A1 63 30 0C 3D F4 21 03 D7 45 F2 02 33 00 00 1C 00 34 61 00 35 06 40 00 3A 41 5C 34 42 16 0E 43 08 53 00 00 60 00 00 61 00 00 C0 9E 03 0F 00 C1 FA 55 2C 1C C4 00 DB BA 71 15 00 04 F8 09 10 04 47 20 3C 50 A1 63 30 0C 3D F4 21 03 D7 45 F2 02 33 00 00 1C 00 34 61 00 35 06 40 00 3A 41 93 38 42 16 0E 43 08 53 5B 38 60 00 00 61 00 00 C0 9E 03 0F 00 C1 FA 55 2C 1C C4 00 DB BA 71 15 00 F1 68",
                new TrackerMessage[]
                {
                    new()
                    {
                        TrId = 2552,
                        TrackerDateTime = new DateTime(638071130320000000),
                        SatNumber = 12,
                        CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
                        Longitude = 52556861 / (double)1000000,
                        Latitude = 49432023 / (double)1000000,
                        Speed = 0,
                        Direction = 28 / (double)10,
                        Height = 97,
                        FuelLevel = 100,
                        CoolantTemperature = 45,
                        EngineSpeed = 901,
                        Tags = new List<MessageTag>
                        {
                            new MessageTagInteger
                            {
                                TrackerTagId = 5,
                                Value = 18181
                            },
                            new MessageTagByte
                            {
                                TrackerTagId = 10,
                                Value = 6
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 11,
                                Value = 14848
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 12,
                                Value = 13404
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 13,
                                Value = 3606
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 14,
                                Value = 8
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 20,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 23,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 24,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 25,
                                Value = 983966
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 28,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 34,
                                Value = 1405370
                            }
                        }
                    },
                    new()
                    {
                        TrId = 2552,
                        TrackerDateTime = new DateTime(638071130200000000),
                        SatNumber = 12,
                        CoordCorrectness = CoordCorrectnessEnum.CorrectGps,
                        Longitude = 52556861 / (double)1000000,
                        Latitude = 49432023 / (double)1000000,
                        Speed = 0,
                        Direction = 28 / (double)10,
                        Height = 97,
                        FuelLevel = 100,
                        CoolantTemperature = 45,
                        EngineSpeed = 901,
                        Tags = new List<MessageTag>
                        {
                            new MessageTagInteger
                            {
                                TrackerTagId = 5,
                                Value = 18182
                            },
                            new MessageTagByte
                            {
                                TrackerTagId = 10,
                                Value = 6
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 11,
                                Value = 14848
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 12,
                                Value = 13404
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 13,
                                Value = 3606
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 14,
                                Value = 8
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 20,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 23,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 24,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 25,
                                Value = 983966
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 28,
                                Value = 0
                            },
                            new MessageTagInteger
                            {
                                TrackerTagId = 34,
                                Value = 1405370
                            }
                        }
                    }
                }
            },
        };

    [Theory, MemberData(nameof(Data))]
    public void ParseMessage_ValidTagsValues_ShouldParseCorrectly(
        string link, string str, TrackerMessage[] expected)
    {
        _testOutputHelper.WriteLine(link);
        _testOutputHelper.WriteLine(str);

        var results = new List<TrackerMessage>();
        var parser = SetupGalileoskyMessageParser(results);

        var message = str.Split()
            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
            .ToArray();

        parser.ParseMessage(message, Guid.NewGuid());

        Assert.Equal(expected.Length, results.Count);

        for (var i = 0; i < expected.Length; i++)
        {
            Assert.NotNull(results[i]);

            Assert.Equal(expected[i].Imei, results[i].Imei);
            Assert.Equal(expected[i].TrId, results[i].TrId);
            Assert.Equal(expected[i].CoordCorrectness, results[i].CoordCorrectness);
            Assert.Equal(expected[i].Direction, results[i].Direction);
            Assert.Equal(expected[i].Height, results[i].Height);
            Assert.Equal(expected[i].Longitude, results[i].Longitude);
            Assert.Equal(expected[i].Latitude, results[i].Latitude);
            Assert.Equal(expected[i].Speed, results[i].Speed);
            Assert.Equal(expected[i].EngineSpeed, results[i].EngineSpeed);
            Assert.Equal(expected[i].CoolantTemperature, results[i].CoolantTemperature);
            Assert.Equal(expected[i].SatNumber, results[i].SatNumber);
            Assert.Equal(expected[i].TrackerDateTime, results[i].TrackerDateTime);
            Assert.Equal(expected[i].FuelLevel, results[i].FuelLevel);

            Assert.Equal(expected[i].Tags.Count, results[i].Tags.Count);
        }
    }

    public static IEnumerable<object[]> Files =>
        new List<object[]>
        {
            new object[]
            {
                "[BIO-81] Расшифровка_пакет_а_JD_с_CAN_данными.docx",
                "./GalileoskyTests/TestCases/1-message.txt",
                "./GalileoskyTests/TestCases/1-result.json"
            },
            new object[]
            {
                "[BIO-81] Расшифровка_пакета_сигнализация.docx",
                "./GalileoskyTests/TestCases/2-message.txt",
                "./GalileoskyTests/TestCases/2-result.json"
            },
            new object[]
            {
                "[BIO-81] Расшифровка пакетов 3 легковые.docx",
                "./GalileoskyTests/TestCases/3-message.txt",
                "./GalileoskyTests/TestCases/3-result.json"
            },
            new object[]
            {
                "[BIO-81] Расшифровка пакетов 2 легковые.docx",
                "./GalileoskyTests/TestCases/4-message.txt",
                "./GalileoskyTests/TestCases/4-result.json"
            }
        };

    [Theory, MemberData(nameof(Files))]
    public void ParseMessage_ValidTagsValuesFromFile_ShouldParseCorrectly(
        string title, string messagePath, string expectedPath)
    {
        _testOutputHelper.WriteLine(title);

        var bytes = File.ReadAllLines(messagePath)
            .SelectMany(x => x.Split())
            .Select(x => byte.Parse(x, NumberStyles.HexNumber))
            .ToArray();

        var results = new List<TrackerMessage>();
        var parser = SetupGalileoskyMessageParser(results);

        parser.ParseMessage(bytes, Guid.NewGuid());

        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter> { new BitArrayConverter() }
        };

        var str = JsonConvert.SerializeObject(results, settings);
        File.WriteAllText(expectedPath, str);

        //var str = File.ReadAllText(expectedPath);
        var expected = JsonConvert.DeserializeObject<List<TrackerMessage>>(str, settings);

        Assert.NotNull(expected);
        Assert.Equal(expected!.Count, results.Count);

        for (var i = 0; i < expected.Count; i++)
        {
            Assert.NotNull(results[i]);

            Assert.Equal(expected[i].Imei, results[i].Imei);
            Assert.Equal(expected[i].TrId, results[i].TrId);
            Assert.Equal(expected[i].CoordCorrectness, results[i].CoordCorrectness);
            Assert.Equal(expected[i].Direction, results[i].Direction);
            Assert.Equal(expected[i].Height, results[i].Height);
            Assert.Equal(expected[i].Longitude, results[i].Longitude);
            Assert.Equal(expected[i].Latitude, results[i].Latitude);
            Assert.Equal(expected[i].Speed, results[i].Speed);
            Assert.Equal(expected[i].EngineSpeed, results[i].EngineSpeed);
            Assert.Equal(expected[i].CoolantTemperature, results[i].CoolantTemperature);
            Assert.Equal(expected[i].SatNumber, results[i].SatNumber);
            Assert.Equal(expected[i].TrackerDateTime, results[i].TrackerDateTime);
            Assert.Equal(expected[i].FuelLevel, results[i].FuelLevel);

            Assert.Equal(expected[i].Tags.Count, results[i].Tags.Count);
        }
    }

    #region SetupTools

    private static GalileoskyMessageParser SetupGalileoskyMessageParser(ICollection<TrackerMessage> results)
    {
        var messageRepo = SetupMessageRepository(results);
        var tagRepo = SetupTagRepository();
        var logStub = new Mock<ILogger<GalileoskyMessageParser>>();

        return new GalileoskyMessageParser(messageRepo, tagRepo, logStub.Object);
    }

    private static ITrackerMessageRepository SetupMessageRepository(ICollection<TrackerMessage> list)
    {
        var keyValueProviderMock = new KeyValueProviderMock<TrackerMessage, int>(list);
        var vehicleQueryProviderMock = new QueryableProviderMock<TrackerMessage>(list);
        var unitOfWorkFactoryMock = new MessagesDBContextUnitOfWorkFactoryMock();
        var tagStub = new Mock<ITrackerTagRepository>();

        var repo = new TrackerMessageRepository(
            keyValueProviderMock,
            vehicleQueryProviderMock,
            unitOfWorkFactoryMock,
            tagStub.Object);

        return repo;
    }

    private static IProtocolTagRepository SetupTagRepository()
    {
        var tagStub = new Mock<IProtocolTagRepository>();

        var trackerTags = TagsSeed.TrackerTags
            .ToDictionary(x => x.Id);

        var protocolTags = TagsSeed.ProtocolTags
            .Where(x => x.TrackerType == TrackerTypeEnum.GalileoSkyV50)
            .Select(x =>
            {
                x.Tag = trackerTags[x.TagId];
                return x;
            })
            .ToArray();

        tagStub.Setup(x => x.GetTagsForTrackerType(TrackerTypeEnum.GalileoSkyV50))
            .Returns(protocolTags);

        return tagStub.Object;
    }

    #endregion
}

public class BitArrayConverter : JsonConverter<BitArray>
{
    public override void WriteJson(JsonWriter writer, BitArray? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        var byteArray = new byte[value.Length / 8 + 1];
        value.CopyTo(byteArray, 0);
        writer.WriteValue(System.Text.Encoding.UTF8.GetString(byteArray));
    }

    public override BitArray? ReadJson(JsonReader reader, Type objectType, BitArray? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.Value is null)
        {
            return null;
        }

        var str = (string)reader.Value;
        var byteArray = System.Text.Encoding.UTF8.GetBytes(str);
        return new BitArray(byteArray);
    }
}