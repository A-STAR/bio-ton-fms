using BioTonFMS.Infrastructure.MessageBus;
using Microsoft.AspNetCore.Connections;
using System.Net;

namespace BioTonFMS.TrackerTcpServer
{
    public class GalileoTrackerConnectionHandler : ConnectionHandler
    {
        private readonly ILogger<GalileoTrackerConnectionHandler> _logger;
        private IMessageBus _messageBus;

        public GalileoTrackerConnectionHandler(
                ILogger<GalileoTrackerConnectionHandler> logger,
                IMessageBus messageBus
            )
        {
            _logger = logger;
            _messageBus = messageBus;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            _logger.LogInformation(connection.ConnectionId + " connected");

            var ipEndPoint = connection.RemoteEndPoint as IPEndPoint;
            if (ipEndPoint != null)
            { 
                _logger.LogInformation($"Получен запрос от {ipEndPoint.Address}:{ipEndPoint.Port}");
            }
            List<byte> message = new();
            while (true)
            {
                var result = await connection.Transport.Input.ReadAsync();
                var buffer = result.Buffer;

                foreach (var segment in buffer)
                {
                    await connection.Transport.Output.WriteAsync(segment);
                    message.AddRange(segment.ToArray());
                }

                if (result.IsCompleted)
                {
                    _messageBus.Publish(message.ToArray());
                    break;
                }

                connection.Transport.Input.AdvanceTo(buffer.End);
            }

            _logger.LogInformation(message: connection.ConnectionId + " disconnected");
        }
    }
}
