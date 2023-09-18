using BioTonFMS.Common.Constants;
using BioTonFMS.Domain;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure.MessageBus;
using System.Collections.Concurrent;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;

public class GalileoskyProtocolMessageHandler : IProtocolMessageHandler, IPublisherConfirmsHandler
{
    private readonly IMessageBus _rawTrackerMessageBus;
    private readonly ILogger<GalileoskyProtocolMessageHandler> _logger;

    const int confirmDelayMs = 50;
    const int confirmTimeoutMs = 1000;

    private readonly ConcurrentDictionary<ulong, ulong> _outstandingConfirms = new ConcurrentDictionary<ulong, ulong>();

    private readonly DateTime _startTime = DateTime.UtcNow;
    private long _messagesReceived = 0;
    private List<DateTime> lastMinuteMessages = new List<DateTime>();
    private List<DateTime> last5MinutesMessages = new List<DateTime>();

    public GalileoskyProtocolMessageHandler(
        Func<MessgingBusType, IMessageBus> busResolver,
        ILogger<GalileoskyProtocolMessageHandler> logger)
    {
        _rawTrackerMessageBus = busResolver(MessgingBusType.RawTrackerMessages);
        _rawTrackerMessageBus.SetPublisherConfirmsHandler(this);
        _logger = logger;
    }

    public async Task<byte[]> HandleMessage(byte[] message, IPAddress ip, int port)
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
            var publishNumber = _rawTrackerMessageBus.Publish(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(raw)));
            _logger.LogInformation("Сообщение отправлено. SeqNum = {SeqNum} Len = {Length} PackageUID = {PackageUID}", publishNumber, message.Length, raw.PackageUID);
            _outstandingConfirms.TryAdd(publishNumber, publishNumber);

            var confirmTaskFinished = false;
            var confirmTask = Task.Run(async () =>
            {
                while (!confirmTaskFinished)
                {
                    await Task.Delay(confirmDelayMs);

                    if (!IsOutstanding(publishNumber))
                    {
                        _logger.LogDebug("Сообщение {PublishNumber} опубликовано", publishNumber);
                        break;
                    }
                }
            });

            var timeoutTask = Task.Delay(confirmTimeoutMs);
            var success = await Task.WhenAny(confirmTask, timeoutTask) == confirmTask;
            confirmTaskFinished = true;

            if (!success)
            {
                counted = 0;
                _logger.LogDebug("Нет подтверждения отправки сообщения {publishNumber}, вовращаем ошибку CRC, чтобы трекер повторил сообщение", publishNumber);
            }
        }
        else
        {
            _logger.LogDebug("Ошибка проверки CRC. Ожидается {Expected} насчитано {Calculated}. Сообщение не опубликовано", received.ToString("X"), counted.ToString("X"));
        }

        byte[] response = GetResponseForTracker(counted);

        _logger.LogDebug("Текст ответа {Response}", string.Join(' ', response.Select(x => x.ToString("X"))));

        // статистика
        _messagesReceived++;
        var now = DateTime.UtcNow;
        lastMinuteMessages.Add(now);
        last5MinutesMessages.Add(now);
        CleanPeriodWindow(lastMinuteMessages, 60);
        CleanPeriodWindow(last5MinutesMessages, 300);
        var totalTime = DateTime.UtcNow - _startTime;
        _logger.LogInformation("Сообщений всего {Total} за последнюю минуту {MessagesPerMinutes} за последние 5 минут {MessagesPer5Minutes}", _messagesReceived, lastMinuteMessages.Count, last5MinutesMessages.Count);
        _logger.LogInformation("Период приёма {Period} мин. рейт {Rate} в сек.", string.Format("{0,6:f}", totalTime.TotalMinutes), string.Format("{0,5:f}", _messagesReceived / totalTime.TotalSeconds));
        return response;
    }

    private void CleanPeriodWindow(List<DateTime> messages, int periodSecs)
    {
        var now = DateTime.UtcNow;
        foreach(var moment in messages.ToArray()) 
        { 
            if (moment < now.AddSeconds(-periodSecs))
            {
                messages.Remove(moment);
            }
        }
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

    private bool IsOutstanding(ulong publishNumber)
    {
        _logger.LogTrace("IsOutstanding = {Keys}", string.Join(' ', _outstandingConfirms.Keys.Select(x => x.ToString())));
        return _outstandingConfirms.ContainsKey(publishNumber);
    }

    public void PublisherConfirm_Acked(ulong deliveryTag, bool multiple)
    {
        _logger.LogDebug("Подтверждение для {deliveryTag} {multiple}", deliveryTag, multiple);
        CleanOutstandingConfirms(deliveryTag, multiple);
    }

    public void PublisherConfirm_Nacked(ulong deliveryTag, bool multiple)
    {
        _outstandingConfirms.TryGetValue(deliveryTag, out _);
        _logger.LogDebug("Сообщение было отвергнуто шиной данных. Sequence number: {deliveryTag}, multiple: {multiple}", deliveryTag, multiple);
    }

    private void CleanOutstandingConfirms(ulong deliveryTag, bool multiple)
    {
        if (multiple)
        {
            var confirmed = _outstandingConfirms.Where(k => k.Key <= deliveryTag);
            foreach (var entry in confirmed)
            {
                _outstandingConfirms.TryRemove(entry.Key, out _);
            }
        }
        else
        {
            _outstandingConfirms.TryRemove(deliveryTag, out _);
        }
        _logger.LogTrace("After CleanOutstandingConfirms = {Keys}", string.Join(' ', _outstandingConfirms.Keys.Select(x => x.ToString())));
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