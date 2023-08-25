using BioTonFMS.Common.Testable;
using BioTonFMS.Domain.Messaging;
using BioTonFMS.Infrastructure;
using BioTonFMS.Infrastructure.Utils.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BioTonFMS.TrackerMessageHandler.Retranslation;

public class Retranslator : IRetranslator
{
    private readonly ILogger<Retranslator> _logger;
    private readonly Factory<ITcpClient> _clientFactory;
    private readonly RetranslatorOptions _options;
    //private readonly ITcpClient _client;

    // список TCP слиентов для каждого адреса с которого приходят пакеты
    private readonly Dictionary<string, ITcpClient> _clientDictionary = new Dictionary<string, ITcpClient>();
    private ITcpClient _client;

    public Retranslator(ILogger<Retranslator> logger, IOptions<RetranslatorOptions> options, Factory<ITcpClient> clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _options = options.Value;
    }

    public async Task Retranslate(byte[] binaryMessage)
    {
        // Проверим есть ли протухшие TcpClient и удалим их
        RemoveOutdatedTcpClients();

        string messageText = Encoding.UTF8.GetString(binaryMessage);
        RawTrackerMessage rawMessage = JsonSerializer.Deserialize<RawTrackerMessage>(messageText)
            ?? throw new ArgumentException("Невозможно разобрать сырое сообщение", messageText);

        byte[] data = rawMessage.RawMessage;
        string messageFrom = $"{rawMessage.IpAddress}:{rawMessage.Port}";

        if (_clientDictionary.ContainsKey(messageFrom))
        {
            _client = _clientDictionary[messageFrom];
        }
        else
        {
            _client = _clientFactory.Invoke();
            _clientDictionary.Add(messageFrom, _client);
        }

        if (!_client.IsConnected())
        {
            try
            {
                await _client.ConnectAsync(_options.Host, _options.Port);
                _logger.LogInformation("Установлено соединение по адресу {Host}:{Port} для {MessageFrom}",
                    _options.Host, _options.Port, messageFrom);
            }
            catch (Exception e)
            {
                _logger.LogError("Ошибка при открытии соединения по адресу {Host}:{Port} для {MessageFrom} - {Message}",
                    _options.Host, _options.Port, messageFrom, e.Message);
                throw new RetranslatioException();
            }
        }

        var stream = _client.GetStream();

        try
        {
            await stream.WriteAsync(data);
            _logger.LogDebug("Отправлено для {MessageFrom}: '{Message}'",
                messageFrom, string.Join(' ', data.Select(x => x.ToString("X"))));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при отправке сообщения для {MessageFrom} {Message}: {Exception}",
                messageFrom, string.Join(' ', data.Select(x => x.ToString("X"))), e.Message);
            // Попытка переоткрыть соединение
            try
            {
                await _client.ConnectAsync(_options.Host, _options.Port);
                _logger.LogInformation("Установлено повторное соединение по адресу {Host}:{Port} для {MessageFrom}",
                    _options.Host, _options.Port, messageFrom);
                try
                {
                    await stream.WriteAsync(data);
                    _logger.LogDebug("Отправлено для {MessageFrom}: '{Message}'",
                        messageFrom, string.Join(' ', data.Select(x => x.ToString("X"))));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при повторной отправке сообщения для {MessageFrom} {Message}: {Exception}",
                        messageFrom, string.Join(' ', data.Select(x => x.ToString("X"))), e.Message);
                    _clientDictionary.Remove(messageFrom);
                    throw new RetranslatioException();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при открытии повторного соединения по адресу {Host}:{Port} для {MessageFrom} - {Message}",
                    _options.Host, _options.Port, messageFrom, ex.Message);
                _clientDictionary.Remove(messageFrom);
                throw new RetranslatioException();
            }
        }

        try
        {
            var respBuf = new byte[10];

            var readCount = 0;
            var readTask = Task.Run(async () =>
            {
                readCount = await stream.ReadAsync(respBuf.AsMemory(0, 10));
            });
            var timeoutTask = Task.Delay(_options.TimeoutSeconds * 1000);
            var success = await Task.WhenAny(readTask, timeoutTask) == readTask;

            if (!success)
            {
                _logger.LogError("Время ожидания истекло при отправке сообщения {Message} для {MessageFrom}",
                    messageFrom, string.Join(' ', data.Select(x => x.ToString("X"))));
                return;
            }

            var responseData = string.Join(' ', respBuf.Take(readCount)
                .Select(x => x.ToString("X")));

            _logger.LogDebug("Получено: '{Response}'", responseData);
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при получении ответа на сообщение {Message}: {Exception}",
                string.Join(' ', data.Select(x => x.ToString("X"))), e.Message);
            throw new RetranslatioException();
        }
    }

    private void RemoveOutdatedTcpClients()
    {
        foreach (string? key in _clientDictionary.Keys.ToArray())
        {
            var client = _clientDictionary[key];
            var clientLastUsed = client.LastUsed();
            if (clientLastUsed.AddSeconds(_options.TcpClientMaxIdleTimeSeconds) < SystemTime.Now)
            {
                client.Dispose();
                _clientDictionary.Remove(key);
                _logger.LogDebug("Удалено не используемое соединение {key}", key);
            }
        }
    }
}