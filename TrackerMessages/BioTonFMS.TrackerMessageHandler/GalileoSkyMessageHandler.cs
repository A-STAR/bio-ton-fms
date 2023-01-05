using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text;

namespace BioTonFMS.TrackerMessageHandler
{
    public class GalileoSkyMessageHandler
    {
        private ILogger<GalileoSkyMessageHandler> _logger;

        public GalileoSkyMessageHandler(ILogger<GalileoSkyMessageHandler> logger)
        {
            _logger = logger;
        }

        public void EventHandler(object? sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _logger.LogInformation("Получено сообщение {message}", message);
        }
    }
}
