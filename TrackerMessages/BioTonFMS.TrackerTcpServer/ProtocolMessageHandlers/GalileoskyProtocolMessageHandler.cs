using System.Text;
using System.Text.Json;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.MessageBus;

namespace BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;

public class GalileoskyProtocolMessageHandler : IProtocolMessageHandler
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<GalileoskyProtocolMessageHandler> _logger;

    public GalileoskyProtocolMessageHandler(
        IMessageBus messageBus,
        ILogger<GalileoskyProtocolMessageHandler> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    public byte[] HandleMessage(byte[] message)
    {
        _logger.LogInformation("Получено сообщение длиной {Len} байт", message.Length);
        _logger.LogDebug("Текст сообщения {Message}", string.Join(' ', message.Select(x => x.ToString("X"))));
        var counted = GetCrc(message[..^2], message.Length - 2);

        var received = BitConverter.ToUInt16(message[^2..], 0);

        if (counted == received)
        {
            var raw = new RawTrackerMessage
            {
                RawMessage = message,
                TrackerType = TrackerTypeEnum.GalileoSkyV50,
                PackageUID = Guid.NewGuid()
            };
            _messageBus.Publish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(raw)));
            _logger.LogInformation("Сообщение опубликовано. Len = {Length}", message.Length);
        }

        return GetResponseForTracker(counted);
    }

    public int GetPacketLength(byte[] message)
    {
        // Определяем длину данных. Она хранится во втором и третьем байте
        var lenRaw = BitConverter.ToUInt16(message[1..3], 0);
        // Чтобы получить длину пакета нужно замаскировать старший бит
        var dataLen = lenRaw & 0x7FFF;

        // Общая длина пакета = 3 (заголовок) + длина данных + 2 (SRC)
        return dataLen + 5;
    }

    private static byte[] GetResponseForTracker(ushort crc)
    {
        var bytes = BitConverter.GetBytes(crc);

        if (!BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return new byte[]
        {
            0x02, // Заголовок
            bytes[0], // Контрольная сумма
            bytes[1] // полученного пакета 
        };
    }

    private static ushort GetCrc(byte[] buf, int len)
    {
        ushort crc = 0xFFFF;

        for (var pos = 0; pos < len; pos++)
        {
            crc ^= buf[pos];

            for (var i = 8; i != 0; i--)
            {
                if ((crc & 0x0001) != 0)
                {
                    crc >>= 1;
                    crc ^= 0xA001;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }

        return crc;
    }
}