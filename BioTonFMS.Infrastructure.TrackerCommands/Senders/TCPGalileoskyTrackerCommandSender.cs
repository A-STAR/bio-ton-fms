using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using BioTonFMS.Common.Constants;
using BioTonFMS.Common.Settings;
using BioTonFMS.Common.Testable;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.MessageBus;
using BioTonFMS.TrackerProtocolSpecific.CommandCodecs;
using BioTonFMS.TrackerProtocolSpecific.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BioTonFMS.TrackerProtocolSpecific.Senders;

public class TcpGalileoskyTrackerCommandSender : ITrackerCommandSender
{
    private readonly TimeSpan _validnessThreshold;
    private readonly ICommandCodec _codec;
    private readonly ILogger<TcpGalileoskyTrackerCommandSender> _logger;
    private readonly IMessageBus _trackerCommandSendBus;

    public TcpGalileoskyTrackerCommandSender(IOptions<TrackerOptions> options,
        Func<TrackerTypeEnum, ICommandCodec> codec,
        ILogger<TcpGalileoskyTrackerCommandSender> logger,
        Func<MessgingBusType, IMessageBus> busResolver)
    {
        _validnessThreshold = TimeSpan.FromMinutes(options.Value.TrackerAddressValidMinutes);
        _codec = codec(TrackerTypeEnum.GalileoSkyV50);
        _logger = logger;
        _trackerCommandSendBus = busResolver.Invoke(MessgingBusType.TrackerCommandsSend);
    }

    public void Send(TrackerCommand command)
    {
        if (command.Tracker == null)
        {
            throw new ArgumentException("Не указан трекер для отправки команды");
        }

        if (!command.Tracker.Port.HasValue || command.Tracker.IpAddress is null || !command.Tracker.LastMessageReceived.HasValue ||
            SystemTime.UtcNow.Subtract(command.Tracker.LastMessageReceived.Value) > _validnessThreshold)
        {
            throw new NotValidTrackerAddressException();
        }

        var msg = new TrackerCommandMessage
        {
            //Imei = command.Tracker.Imei,
            //ExternalId = command.Tracker.ExternalId,
            IpAddress = command.Tracker.IpAddress,
            Port = command.Tracker.Port.Value,
            SentDateTime = command.SentDateTime,
            CommandId = command.Id,
            EncodedCommand = _codec.EncodeCommand(command.Tracker, command.Id, command.CommandText)
        };
        _trackerCommandSendBus.Publish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msg)));
        _logger.LogDebug("Опубликовано сообщение TrackerCommandMessage = {msg}", msg);
    }
    /*
    public (string ResponseText, byte[] ResponseBinaryInfo) SendOld(Tracker tracker, string commandText)
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
            byte[] data = _codec.EncodeCommand(tracker, commandText);

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

        return _codec.DecodeCommand(resp);
    }*/

}