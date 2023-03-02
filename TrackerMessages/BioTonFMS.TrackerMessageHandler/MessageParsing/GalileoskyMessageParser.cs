using System.Collections;
using BioTonFMS.Domain;
using BioTonFMS.Domain.TrackerMessages;
using BioTonFMS.Infrastructure.EF.Repositories.ProtocolTags;
using BioTonFMS.Infrastructure.EF.Repositories.TrackerMessages;
using Microsoft.Extensions.Logging;

namespace BioTonFMS.TrackerMessageHandler.MessageParsing;

public class GalileoskyMessageParser : IMessageParser
{
    private const int ImeiCode = 0x03;
    private const int TrackerIdCode = 0x04;
    private const int TrackerDateTimeCode = 0x20;
    private const int CoordsStructCode = 0x30;
    private const int VelocityStructCode = 0x33;
    private const int HeightStructCode = 0x34;
    private const int CanLogStructCode = 0xC1;

    private const int CheckSumLength = 2;
    private const int HeaderLength = 3;
    
    private readonly IProtocolTagRepository _protocolTagRepository;
    private readonly ITrackerMessageRepository _messageRepository;
    private readonly ILogger<GalileoskyMessageParser> _logger;

    public GalileoskyMessageParser(ITrackerMessageRepository messageRepository,
        IProtocolTagRepository protocolTagRepository, ILogger<GalileoskyMessageParser> logger)
    {
        _messageRepository = messageRepository;
        _protocolTagRepository = protocolTagRepository;
        _logger = logger;
    }

    public void ParseMessage(byte[] binaryPackage, Guid packageUID)
    {
        if (_messageRepository.ExistsByUID(packageUID)) return;

        var i = 0;
        i += HeaderLength;

        var tags = _protocolTagRepository.GetTagsForTrackerType(TrackerTypeEnum.GalileoSkyV50)
            .ToDictionary(x => x.ProtocolTagCode);

        var message = new TrackerMessage
        {
            PackageUID = packageUID
        };

        while (i < binaryPackage.Length - CheckSumLength)
        {
            if (!tags.TryGetValue(binaryPackage[i], out var tag))
            {
                _logger.LogError("Тег с кодом {Code} не найден, позиция в сообщении {Position}, сообщение {Message}",
                    binaryPackage[i], i, string.Join(' ', binaryPackage.Select(x => x.ToString("X"))));
                return;
            }

            i++;

            switch (tag.ProtocolTagCode)
            {
                case ImeiCode:
                    if (!string.IsNullOrEmpty(message.Imei))
                    {
                        _messageRepository.Add(message);
                        message = new TrackerMessage
                        {
                            PackageUID = packageUID
                        };
                    }

                    message.Imei = binaryPackage[i..(i + tag.Size)].ParseToString();
                    break;
                case TrackerIdCode:
                    if (message.TrId != 0)
                    {
                        _messageRepository.Add(message);
                        message = new TrackerMessage
                        {
                            PackageUID = packageUID
                        };
                    }

                    message.TrId = binaryPackage[i..(i + tag.Size)].ParseToInt();
                    break;
                case TrackerDateTimeCode:
                    message.TrackerDateTime = binaryPackage[i..(i + tag.Size)].ParseToDateTime();
                    break;
                case CoordsStructCode:
                    var coords = binaryPackage[i..(i + tag.Size)].ParseCoords();
                    message.CoordCorrectness = coords.Correctness;
                    message.SatNumber = coords.SatNumber;
                    message.Longitude = coords.Longitude;
                    message.Latitude = coords.Latitude;
                    break;
                case VelocityStructCode:
                    var velocity = binaryPackage[i..(i + tag.Size)].ParseVelocity();
                    message.Speed = velocity.Speed;
                    message.Direction = velocity.Direction;
                    break;
                case HeightStructCode:
                    message.Height = binaryPackage[i..(i + tag.Size)].ParseToInt();
                    break;
                case CanLogStructCode:
                    var data = binaryPackage[i..(i + tag.Size)].ParseCanLog();
                    message.FuelLevel = data.FuelLevel;
                    message.CoolantTemperature = data.СoolantTemperature;
                    message.EngineSpeed = data.EngineSpeed;
                    break;
                default:
                    message.Tags.Add(CreateMessageTag(tag.Tag, binaryPackage[i..(i + tag.Size)]));
                    break;
            }

            i += tag.Size;
        }

        _messageRepository.Add(message);
    }

    private static MessageTag CreateMessageTag(TrackerTag trackerTag, byte[] value) =>
        trackerTag.DataType switch
        {
            TagDataTypeEnum.Integer => new MessageTagInteger
            {
                TrackerTagId = trackerTag.Id,
                Value = value.ParseToInt()
            },
            TagDataTypeEnum.Double => new MessageTagDouble
            {
                TrackerTagId = trackerTag.Id,
                Value = value.ParseToDouble()
            },
            TagDataTypeEnum.Boolean => new MessageTagBoolean
            {
                TrackerTagId = trackerTag.Id,
                Value = value.Any(x => x != 0)
            },
            TagDataTypeEnum.String => new MessageTagString
            {
                TrackerTagId = trackerTag.Id,
                Value = value.ParseToString()
            },
            TagDataTypeEnum.DateTime => new MessageTagDateTime
            {
                TrackerTagId = trackerTag.Id,
                Value = value.ParseToDateTime()
            },
            TagDataTypeEnum.Bits => new MessageTagBits
            {
                TrackerTagId = trackerTag.Id,
                Value = new BitArray(value)
            },
            TagDataTypeEnum.Byte => new MessageTagByte
            {
                TrackerTagId = trackerTag.Id,
                Value = value[0]
            },
            TagDataTypeEnum.Struct or _ =>
                throw new ArgumentOutOfRangeException(nameof(trackerTag))
        };
}