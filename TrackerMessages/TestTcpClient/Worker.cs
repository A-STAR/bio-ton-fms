using System.Globalization;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestTcpTrackerClient;

public class Worker : BackgroundService
{
    private readonly ClientOptions _options;
    private readonly ClientParams _parameters;
    private readonly ILogger<Worker> _logger;

    public Worker(ClientOptions options, ClientParams parameters, ILogger<Worker> logger)
    {
        _options = options;
        _parameters = parameters;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var client = new TcpClient(_options.Host, _options.Port);
        var stream = client.GetStream();

        if (_parameters.ScriptPath != null)
        {
            var lines = File.ReadAllLines(_parameters.ScriptPath);
            foreach (var l in lines)
            {
                var paths = l.Split(' ');
                if (paths.Length != 2)
                {
                    _logger.LogWarning("Строка {Line} не соответствует формату", l);
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

    private void SendRequest(string input, string output, Stream stream)
    {
        if (!File.Exists(input))
        {
            _logger.LogWarning("Файл {Path} не существует", input);
            return;
        }

        try
        {
            var message = File.ReadAllText(input);
            var data = message.Split(' ', '\n')
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                .ToArray();

            stream.Write(data, 0, data.Length);
            _logger.LogInformation("Отправлено: {Message}", message);
        }
        catch (Exception e)
        {
            _logger.LogWarning("Ошибка {Exception} при парсинге или отправке сообщения из файла {Path}",
                e.Message, input);
        }
        
        var respBuf = new byte[10];
        var readCount = stream.Read(respBuf, 0, 10);
        var responseData = string.Join(' ', respBuf.Take(readCount)
            .Select(x => x.ToString("X")));

        _logger.LogInformation("Получено: {Response}", responseData);
        File.WriteAllText(output, responseData);
    }
}