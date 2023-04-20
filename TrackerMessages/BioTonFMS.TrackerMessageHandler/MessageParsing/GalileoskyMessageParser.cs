using System.Collections;
using System.Diagnostics;
using BioTonFMS.Common.Constants;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.ProtocolTags;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerTags;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.TrackerMessageHandler.MessageParsing;

public class GalileoskyMessageParser : IMessageParser
{
    private const int ImeiCode = 0x03;
    private const int TrackerIdCode = 0x04;
    private const int TrackerDateTimeCode = 0x20;
    private const int CoordsStructCode = 0x30;
    private const int VelocityStructCode = 0x33;
    private const int AltitudeStructCode = 0x34;
    private const int CanLogStructCode = 0xC1;

    private readonly IProtocolTagRepository _protocolTagRepository;
    private readonly ILogger<GalileoskyMessageParser> _logger;

    public GalileoskyMessageParser(
        IProtocolTagRepository protocolTagRepository, 
        ILogger<GalileoskyMessageParser> logger)
    {
        _protocolTagRepository = protocolTagRepository;
        _logger = logger;
    }

    public IEnumerable<TrackerMessage> ParseMessage(byte[] binaryPackage, Guid packageUid)
    {
        var i = 0;
        i += Galileosky.HeaderLength;

        var tags = _protocolTagRepository.GetTagsForTrackerType(TrackerTypeEnum.GalileoSkyV50)
            .ToDictionary(x => x.ProtocolTagCode);

        var message = new TrackerMessage
        {
            PackageUID = packageUid
        };

        while (i < binaryPackage.Length - Galileosky.CheckSumLength)
        {
            if (!tags.TryGetValue(binaryPackage[i], out ProtocolTag? tag))
            {
                _logger.LogError("Тег с кодом {Code} не найден, позиция в сообщении {Position}, сообщение {Message}, пакет {PackageUID}",
                    binaryPackage[i], i, string.Join(' ', binaryPackage.Select(x => x.ToString("X"))), packageUid);
                yield break;
            }

            i++;

            switch (tag.ProtocolTagCode)
            {
                case ImeiCode:
                    if (!string.IsNullOrEmpty(message.Imei))
                    {
                        yield return message;
                        message = new TrackerMessage
                        {
                            PackageUID = packageUid
                        };
                    }
                    message.Imei = AddMessageTag<MessageTagString>(tag, binaryPackage, i, message).Value;
                    break;
                case TrackerIdCode:
                    if (message.ExternalTrackerId != 0)
                    {
                        yield return message;
                        message = new TrackerMessage
                        {
                            PackageUID = packageUid
                        };
                    }
                    message.ExternalTrackerId = AddMessageTag<MessageTagInteger>(tag, binaryPackage, i, message).Value;
                    break;
                case TrackerDateTimeCode:
                    message.TrackerDateTime = AddMessageTag<MessageTagDateTime>(tag, binaryPackage, i, message).Value;
                    break;
                case CoordsStructCode:
                    var coords = binaryPackage[i..(i + tag.Size)].ParseCoords();
                    message.CoordCorrectness = coords.Correctness;
                    AddMessageTag((int)coords.Correctness, TagsSeed.CorrectnessId, message);
                    message.SatNumber = coords.SatNumber;
                    AddMessageTag(coords.SatNumber, TagsSeed.SatNumberId, message);
                    message.Longitude = coords.Longitude;
                    AddMessageTag(coords.Longitude, TagsSeed.LongitudeId, message);
                    message.Latitude = coords.Latitude;
                    AddMessageTag(coords.Latitude, TagsSeed.LatitudeId, message);
                    break;
                case VelocityStructCode:
                    var velocity = binaryPackage[i..(i + tag.Size)].ParseVelocity();
                    message.Speed = velocity.Speed;
                    AddMessageTag(velocity.Speed, TagsSeed.SpeedId, message);
                    message.Direction = velocity.Direction;
                    AddMessageTag(velocity.Direction, TagsSeed.DirectionId, message);
                    break;
                case AltitudeStructCode:
                    message.Altitude = AddMessageTag<MessageTagInteger>(tag, binaryPackage, i, message).Value;
                    break;
                case CanLogStructCode:
                    var data = binaryPackage[i..(i + tag.Size)].ParseCanLog();
                    message.FuelLevel = data.FuelLevel;
                    AddMessageTag(data.FuelLevel, TagsSeed.FuelLevelId, message);
                    message.CoolantTemperature = data.CoolantTemperature;
                    AddMessageTag(data.CoolantTemperature, TagsSeed.CoolantTemperatureId, message);
                    message.EngineSpeed = data.EngineSpeed;
                    AddMessageTag(data.EngineSpeed, TagsSeed.EngineSpeedId, message);
                    break;
                default:
                    if (tag.Tag is not null)
                        message.Tags.Add(CreateMessageTag(tag.Tag, binaryPackage[i..(i + tag.Size)]));
                    break;
            }

            i += tag.Size;
        }

        yield return message;
    }

    private static void AddMessageTag<TValue>(TValue value, int trackerTagId, TrackerMessage message)
    {
        var messageTag = MessageTag.Create(value);
        messageTag.TrackerTagId = trackerTagId;
        message.Tags.Add(messageTag);
    }

    private static TTagType AddMessageTag<TTagType>(ProtocolTag tag, byte[] binaryPackage, int i, TrackerMessage message)
        where TTagType : MessageTag
    {
        Debug.Assert(tag.Tag is not null);
        
        var messageTag = CreateMessageTag(tag.Tag!, binaryPackage[i..(i + tag.Size)]);
        message.Tags.Add(messageTag);
        return ((TTagType)messageTag);
    }    

    private static MessageTag CreateMessageTag(TrackerTag trackerTag, byte[] value) =>
        trackerTag.DataType switch
        {
            TagDataTypeEnum.Integer => new MessageTagInteger
            {
                TrackerTagId = trackerTag.Id, Value = value.ParseToInt()
            },
            TagDataTypeEnum.Double => new MessageTagDouble
            {
                TrackerTagId = trackerTag.Id, Value = value.ParseToDouble()
            },
            TagDataTypeEnum.Boolean => new MessageTagBoolean
            {
                TrackerTagId = trackerTag.Id, Value = value.Any(x => x != 0)
            },
            TagDataTypeEnum.String => new MessageTagString
            {
                TrackerTagId = trackerTag.Id, Value = value.ParseToString()
            },
            TagDataTypeEnum.DateTime => new MessageTagDateTime
            {
                TrackerTagId = trackerTag.Id, Value = value.ParseToDateTime()
            },
            TagDataTypeEnum.Bits => new MessageTagBits
            {
                TrackerTagId = trackerTag.Id, Value = new BitArray(value)
            },
            TagDataTypeEnum.Byte => new MessageTagByte
            {
                TrackerTagId = trackerTag.Id, Value = value[0]
            },
            TagDataTypeEnum.Struct or _ =>
                throw new ArgumentOutOfRangeException(nameof(trackerTag))
        };
}
