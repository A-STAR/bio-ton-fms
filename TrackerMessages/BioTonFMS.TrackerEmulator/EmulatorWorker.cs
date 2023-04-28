using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Sockets;
using System.Text.Json;

namespace BioTonFMS.TrackerEmulator;

public class EmulatorWorker : BackgroundService
{
    private readonly ClientOptions _options;
    private readonly ClientParams _parameters;
    private readonly ILogger<EmulatorWorker> _logger;

    public EmulatorWorker(IOptions<ClientOptions> options,
        ClientParams parameters, ILogger<EmulatorWorker> logger)
    {
        _options = options.Value;
        _parameters = parameters;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Эмулятор запущен");
        ValidateParams();

        _logger.LogInformation("Параметры корректны");

        using var client = new TcpClient();
        client.ReceiveTimeout = _options.TimeoutSeconds * 1000;

        try
        {
            client.Connect(_options.Host, _options.Port);
            _logger.LogInformation("Установлено соединение по адресу {Host}:{Port}",
                _options.Host, _options.Port);
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при открытии соединения по адресу {Host}:{Port} - {Message}",
                _options.Host, _options.Port, e.Message);
            client.Dispose();
            Environment.Exit(1);
        }

        NetworkStream stream = client.GetStream();

        if (_parameters.ScriptPath is not null)
        {
            foreach (var line in File.ReadAllLines(_parameters.ScriptPath))
            {
                var paths = line.Split(' ');
                await SendRequest(paths[0], paths[1], stream, stoppingToken);
            }

            return;
        }

        if (_parameters is not { MessagePath: not null, ResultPath: not null }) return;

        for (var i = 0; i < _parameters.RepeatCount; i++)
        {
            await SendRequest(_parameters.MessagePath, _parameters.ResultPath, stream, stoppingToken);
            if (stoppingToken.IsCancellationRequested) break;
            Thread.Sleep(_options.DelaySeconds * 1000);
        }

        return;
    }

    private async Task SendRequest(string messageFile, string resultFile, Stream stream, CancellationToken stoppingToken)
    {
        try
        {
            byte[] data = ReadMessageFromFile(messageFile);

            await stream.WriteAsync(data, 0, data.Length, stoppingToken);
            _logger.LogInformation("Отправлено: '{Message}'",
                string.Join(' ', data.Select(x => x.ToString("X"))));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при отправке сообщения из файла {Path}: {Message}",
                messageFile, e.Message);
            return;
        }

        string responseData;

        try
        {
            var respBuf = new byte[10];
            int readCount = await stream.ReadAsync(respBuf, 0, 10, stoppingToken);
            responseData = string.Join(' ', respBuf.Take(readCount)
                .Select(x => x.ToString("X")));

            _logger.LogInformation("Получено: {Response}", responseData);
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при получении ответа на сообщение из файла {Path}: {Message}",
                messageFile, e.Message);
            return;
        }

        try
        {
            File.WriteAllText(resultFile, responseData);
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при записи ответа на сообщение в файла {Path}: {Message}",
                resultFile, e.Message);
        }
    }

    private void ValidateParams()
    {
        if (!string.IsNullOrWhiteSpace(_parameters.ScriptPath))
        {
            if (!File.Exists(_parameters.ScriptPath))
            {
                _logger.LogError("Файл {Path} не существует", _parameters.ScriptPath);
                Environment.Exit(1);
            }

            foreach (string l in File.ReadAllLines(_parameters.ScriptPath))
            {
                string[] paths = l.Split(' ');
                if (paths.Length != 2 ||
                    !File.Exists(paths[0]) ||
                    string.IsNullOrWhiteSpace(paths[1]))
                {
                    _logger.LogError("Ошибка в строке {Line}", l);
                    Environment.Exit(1);
                }

                ValidateInput(paths[0]);
            }

            return;
        }

        if (!string.IsNullOrWhiteSpace(_parameters.MessagePath) &&
            !string.IsNullOrWhiteSpace(_parameters.ResultPath))
        {
            if (!File.Exists(_parameters.MessagePath))
            {
                _logger.LogError("Файл {Path} не существует", _parameters.MessagePath);
                Environment.Exit(1);
            }

            ValidateInput(_parameters.MessagePath);
            return;
        }

        _logger.LogError("Ошибка в параметрах:\n{Params}", JsonSerializer.Serialize(_parameters));
        Environment.Exit(1);
    }

    private void ValidateInput(string inputPath)
    {
        try
        {
            ReadMessageFromFile(inputPath);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при разборе сообщения из файла {Path}", inputPath);
            Environment.Exit(1);
        }
    }

    private static byte[] ReadMessageFromFile(string path) =>
        new FileInfo(path).Extension == ".txt"
            ? File.ReadAllText(path).Replace("\r\n", "\n")
                .Split(' ', '\n')
                .Where(e => e.Length > 0)
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                .ToArray()
            : File.ReadAllBytes(path);
}