using System.Collections;
using System.Diagnostics;
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

    private const int CheckSumLength = 2;
    private const int HeaderLength = 3;

    private readonly IProtocolTagRepository _protocolTagRepository;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ILogger<GalileoskyMessageParser> _logger;

    public GalileoskyMessageParser(
        ITrackerMessageRepository messageRepository,
        IProtocolTagRepository protocolTagRepository,
        ILogger<GalileoskyMessageParser> logger)
    {
        _messageRepository = messageRepository;
        _protocolTagRepository = protocolTagRepository;
        _logger = logger;
    }

    public void ParseMessage(byte[] binaryPackage, Guid packageUid)
    {
        if (_messageRepository.ExistsByUID(packageUid)) return;

        var i = 0;
        i += HeaderLength;

        var tags = _protocolTagRepository.GetTagsForTrackerType(TrackerTypeEnum.GalileoSkyV50)
            .ToDictionary(x => x.ProtocolTagCode);

        var message = new TrackerMessage
        {
            PackageUID = packageUid
        };

        while (i < binaryPackage.Length - CheckSumLength)
        {
            if (!tags.TryGetValue(binaryPackage[i], out ProtocolTag? tag))
            {
                _logger.LogError("Тег с кодом {Code} не найден, позиция в сообщении {Position}, сообщение {Message}, пакет {PackageUID}",
                    binaryPackage[i], i, string.Join(' ', binaryPackage.Select(x => x.ToString("X"))), packageUid);
                return;
            }

            i++;

            switch (tag.ProtocolTagCode)
            {
                case ImeiCode:
                    if (!string.IsNullOrEmpty(message.Imei))
                    {
                        _messageRepository.Add(message);
                        _logger.LogDebug(
                            "Добавлено новое сообщение из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId} Imei={Imei}", packageUid,
                            message.Id, message.TrId, message.Imei);
                        message = new TrackerMessage
                        {
                            PackageUID = packageUid
                        };
                    }
                    message.Imei = AddMessageTag<MessageTagString>(tag, binaryPackage, i, message).Value;
                    break;
                case TrackerIdCode:
                    if (message.TrId != 0)
                    {
                        _messageRepository.Add(message);
                        _logger.LogDebug(
                            "Добавлено новое сообщение из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId} Imei={Imei}", packageUid,
                            message.Id, message.TrId, message.Imei);
                        message = new TrackerMessage
                        {
                            PackageUID = packageUid
                        };
                    }
                    message.TrId = AddMessageTag<MessageTagInteger>(tag, binaryPackage, i, message).Value;
                    break;
                case TrackerDateTimeCode:
                    message.TrackerDateTime = AddMessageTag<MessageTagDateTime>(tag, binaryPackage, i, message).Value;
                    break;
                case CoordsStructCode:
                    var coords = binaryPackage[i..(i + tag.Size)].ParseCoords();
                    message.CoordCorrectness = coords.Correctness;
                    AddMessageTag((int)coords.Correctness, TagsSeed.CoordCorrectnessId, message);
                    message.SatNumber = coords.SatNumber;
                    AddMessageTag(coords.SatNumber, TagsSeed.CoordSatNumberId, message);
                    message.Longitude = coords.Longitude;
                    AddMessageTag(coords.Longitude, TagsSeed.CoordLongitudeId, message);
                    message.Latitude = coords.Latitude;
                    AddMessageTag(coords.Latitude, TagsSeed.CoordLatitudeId, message);
                    break;
                case VelocityStructCode:
                    var velocity = binaryPackage[i..(i + tag.Size)].ParseVelocity();
                    message.Speed = velocity.Speed;
                    AddMessageTag(velocity.Speed, TagsSeed.VelocitySpeedId, message);
                    message.Direction = velocity.Direction;
                    AddMessageTag(velocity.Direction, TagsSeed.VelocityDirectionId, message);
                    break;
                case AltitudeStructCode:
                    message.Altitude = AddMessageTag<MessageTagInteger>(tag, binaryPackage, i, message).Value;
                    break;
                case CanLogStructCode:
                    var data = binaryPackage[i..(i + tag.Size)].ParseCanLog();
                    message.FuelLevel = data.FuelLevel;
                    AddMessageTag(data.FuelLevel, TagsSeed.CanLogFuelLevelId, message);
                    message.CoolantTemperature = data.CoolantTemperature;
                    AddMessageTag(data.CoolantTemperature, TagsSeed.CanLogCoolantTemperatureId, message);
                    message.EngineSpeed = data.EngineSpeed;
                    AddMessageTag(data.EngineSpeed, TagsSeed.CanLogEngineSpeedId, message);
                    break;
                default:
                    if (tag.Tag is not null)
                        message.Tags.Add(CreateMessageTag(tag.Tag, binaryPackage[i..(i + tag.Size)]));
                    break;
            }

            i += tag.Size;
        }

        _messageRepository.Add(message);
        _logger.LogDebug("Добавлено новое сообщение из пакета {PackageUID} Id сообщения {MessageId} TrId={TrId} Imei={Imei}", packageUid,
            message.Id, message.TrId, message.Imei);
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
