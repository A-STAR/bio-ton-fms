using System.Globalization;
using BiotonFMS.IntegrationTests.Extensions;
using Xunit.Abstractions;

namespace BiotonFMS.IntegrationTests.Utils;
internal static class TestUtils
{
    public static int TimeoutSeconds = 15;

    public static async Task<string> SendRequest(ITestOutputHelper _logger, string messageFile, Stream stream, CancellationToken stoppingToken)
    {
        try
        {
            byte[] data = ReadMessageFromFile(messageFile);

            await stream.WriteAsync(data, 0, data.Length, stoppingToken);
            _logger.LogInformation("Отправлено: '{0}'",
                string.Join(' ', data.Select(x => x.ToString("X"))));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при отправке сообщения из файла {0}: {1}",
                messageFile, e.Message);
            throw;
        }

        string responseData;
        try
        {
            var respBuf = new byte[10];

            var readCount = 0;
            var readTask = Task.Run(async () => { readCount = await stream.ReadAsync(respBuf, 0, 10, stoppingToken); });
            var timeoutTask = Task.Delay(TimeoutSeconds * 1000);
            var success = await Task.WhenAny(readTask, timeoutTask) == readTask;
            if (!success)
            {
                throw new Exception("Read timeout");
            }

            responseData = string.Join(' ', respBuf.Take(readCount)
                .Select(x => x.ToString("X")));

            _logger.LogInformation("Получено: {0}", responseData);
            
        }
        catch (Exception e)
        {
            _logger.LogError("Ошибка при получении ответа на сообщение из файла {0}: {1}",
                messageFile, e.Message);
            throw;
        }

        _logger.LogInformation("Ответ:");
        _logger.LogInformation(responseData);
        return responseData;
    }

    public static byte[] ReadMessageFromFile(string path) =>
        new FileInfo(path).Extension == ".txt"
            ? File.ReadAllText(path).Replace("\r\n", "\n")
                .Split(' ', '\n')
                .Where(e => e.Length > 0)
                .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                .ToArray()
            : File.ReadAllBytes(path);

}
