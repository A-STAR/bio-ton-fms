using System.Globalization;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TrackerEmulator;

public class Worker : BackgroundService
{
    private readonly IOptions<ClientOptions> _options;
    private readonly ClientParams _parameters;
    private readonly ILogger<Worker> _logger;

    public Worker(IOptions<ClientOptions> options, ClientParams parameters, ILogger<Worker> logger)
    {
        _options = options;
        _parameters = parameters;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Emulator started");
        using var client = new TcpClient(_options.Value.Host, _options.Value.Port);
        NetworkStream stream = client.GetStream();

        if (_parameters.ScriptPath != null)
        {
            string[] lines = File.ReadAllLines(_parameters.ScriptPath);
            foreach (var line in lines)
            {
                var paths = line.Split(' ');
                if (paths.Length != 2)
                {
                    _logger.LogError("Строка '{Line}' не соответствует формату", line);
                    continue;
                }
                SendRequest(paths[0], paths[1], stream);
            }

            return Task.CompletedTask;
        }

        if (_parameters is { InputPath: { }, OutputPath: { } })
        {
            for (var i = 0; i < _parameters.RepeatCount; i++)
            {
                SendRequest(_parameters.InputPath, _parameters.OutputPath, stream);
            }

            return Task.CompletedTask;
        }

        throw new Exception($"Ошибка в параметрах:\n{JsonSerializer.Serialize(_parameters)}");
    }

    private void SendRequest(string messageFile, string resultFile, Stream stream)
    {
        if (!File.Exists(messageFile))
        {
            _logger.LogError("Файл '{Path}' не существует", messageFile);
            return;
        }

        try
        {
            string message = File.ReadAllText(messageFile);
            byte[] data = message.Replace("\r\n", "\n")
                .Split(' ', '\n')
                .Where(e => e.Length > 0)
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                .ToArray();

            stream.Write(data, 0, data.Length);
            _logger.LogInformation("Отправлено: '{Message}'", message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при парсинге или отправке сообщения из файла {Path}", messageFile);
        }

        var respBuf = new byte[10];
        int readCount = stream.Read(respBuf, 0, 10);
        string responseData = string.Join(' ', respBuf.Take(readCount)
            .Select(x => x.ToString("X")));

        _logger.LogInformation("Получено: {Response}", responseData);
        File.WriteAllText(resultFile, responseData);
    }
}