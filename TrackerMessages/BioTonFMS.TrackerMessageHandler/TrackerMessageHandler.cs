using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.Extensions.Logging;
using System.Text;

namespace BioTonFMS.TrackerMessageHandler
{
    public class TrackerMessageHandler : IBusMessageHandler
    {
        private readonly ILogger<TrackerMessageHandler> _logger;

        public TrackerMessageHandler(ILogger<TrackerMessageHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(byte[] message)
        {
            var messageText = Encoding.UTF8.GetString(message);
            _logger.LogInformation("Получено сообщение {messageText}", messageText);
            return Task.CompletedTask;
        }
    }
}
