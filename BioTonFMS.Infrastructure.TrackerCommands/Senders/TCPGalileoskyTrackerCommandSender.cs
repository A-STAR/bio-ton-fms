using System.Net.Sockets;
using BioTonFMS.Common.Constants;
using BioTonFMS.Common.Settings;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.TrackerCommands.Codecs;
using BioTonFMS.TrackerCommands.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioTonFMS.TrackerCommands.Senders;

public class TcpGalileoskyTrackerCommandSender : ITrackerCommandSender
{
    private readonly TimeSpan _validnessThreshold;
    private readonly ICommandCodec _codec;
    private readonly ILogger<TcpGalileoskyTrackerCommandSender> _logger;

    public TcpGalileoskyTrackerCommandSender(IOptions<TrackerOptions> options,
        Func<TrackerTypeEnum, ICommandCodec> codec,
        ILogger<TcpGalileoskyTrackerCommandSender> logger)
    {
        _validnessThreshold = TimeSpan.FromMinutes(options.Value.TrackerAddressValidMinutes);
        _codec = codec(TrackerTypeEnum.GalileoSkyV50);
        _logger = logger;
    }

    public (string ResponseText, byte[] ResponseBinaryInfo) Send(Tracker tracker, string commandText)
    {
        if (!tracker.Port.HasValue || tracker.IpAddress is null || !tracker.LastMessageReceived.HasValue ||
            SystemTime.UtcNow.Subtract(tracker.LastMessageReceived.Value) > _validnessThreshold)
        {
            throw new NotValidTrackerAddressException();
        }

        using var client = new TcpClient();
        try
        {
            client.Connect(tracker.IpAddress, tracker.Port.Value);
            _logger.LogInformation("Установлено соединение по адресу {Host}:{Port}",
                tracker.IpAddress, tracker.Port.Value);
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при открытии соединения по адресу {Host}:{Port} - {Message}",
                tracker.IpAddress, tracker.Port.Value, e.Message);
            throw;
        }

        using NetworkStream stream = client.GetStream();
        try
        {
            byte[] data = _codec.Encode(tracker, commandText);

            stream.Write(data, 0, data.Length);
            _logger.LogInformation("Отправлено: '{Message}'", commandText);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при отправке команды {Command} трекеру id = {Tracker} " +
                                "по адресу {Host}:{Port}",
                commandText, tracker.Id, tracker.IpAddress, tracker.Port.Value);
            throw;
        }

        byte[] resp;
        try
        {
            var respHead = new byte[Galileosky.HeaderLength];
            int headLength = stream.Read(respHead, 0, respHead.Length);

            if (headLength != Galileosky.HeaderLength ||
                respHead[0] != Galileosky.HeaderTag)
            {
                throw new ParseCommandResponseException("Неверный формат заголовка");
            }

            int bodyLength = BitConverter.ToUInt16(respHead[1..], 0) + Galileosky.CheckSumLength;
            resp = new byte[bodyLength];
            int respLength = stream.Read(respHead, 0, respHead.Length);

            if (respLength != bodyLength)
            {
                throw new ParseCommandResponseException(
                    "Длина ответа не совпадает с заявленной длиной пакета из заголовка");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при получении ответа на сообщение");
            throw;
        }

        ushort countedCrc = Galileosky.GetCrc(resp[..^2], resp.Length - Galileosky.CheckSumLength);
        var receivedCrc = BitConverter.ToUInt16(resp[^2..], 0);

        if (countedCrc != receivedCrc)
        {
            throw new ParseCommandResponseException("Неверная контрольная сумма");
        }

        return _codec.Decode(resp);
    }
}