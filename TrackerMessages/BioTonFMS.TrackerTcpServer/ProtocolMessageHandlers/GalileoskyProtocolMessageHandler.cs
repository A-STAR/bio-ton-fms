using System.Net;
using System.Text;
using System.Text.Json;
using BioTonFMS.Common.Constants;
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

    public byte[] HandleMessage(byte[] message, IPAddress ip, int port)
    {
        _logger.LogInformation("Получено сообщение длиной {Len} байт", message.Length);
        _logger.LogDebug("Текст сообщения {Message}", string.Join(' ', message.Select(x => x.ToString("X"))));
        ushort counted = Galileosky.GetCrc(message[..^2], message.Length - Galileosky.CheckSumLength);

        var received = BitConverter.ToUInt16(message[^2..], 0);

        if (counted == received)
        {
            var raw = new RawTrackerMessage
            {
                RawMessage = message,
                TrackerType = TrackerTypeEnum.GalileoSkyV50,
                PackageUID = Guid.NewGuid(),
                IpAddress = ip.ToString(),
                Port = port
            };
            _messageBus.Publish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(raw)));
            _logger.LogInformation("Сообщение опубликовано. Len = {Length} PackageUID = {PackageUID}", message.Length, raw.PackageUID);
        }
        else
        {
            _logger.LogDebug("Ошибка проверки CRC. Ожидается {Expected} насчитано {Calculated}. Сообщение не опубликовано", received.ToString("X"), counted.ToString("X"));
        }

        byte[] response = GetResponseForTracker(counted);
        _logger.LogDebug("Текст ответа {Response}", string.Join(' ', response.Select(x => x.ToString("X"))));
        return response;
    }

    public int GetPacketLength(byte[] message)
    {
        if (message.Length < 3)
        {
            _logger.LogDebug("Получено слишком короткое сообщение. Len = {Length}", message.Length);
            return -1;
        }
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
}