using System.Net;
using BioTonFMS.Domain;
using BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;
using Microsoft.AspNetCore.Connections;

namespace BioTonFMS.TrackerTcpServer;

public class GalileoTrackerConnectionHandler : ConnectionHandler
{
    private readonly ILogger<GalileoTrackerConnectionHandler> _logger;
    private readonly IProtocolMessageHandler _handler;

    public GalileoTrackerConnectionHandler(
        ILogger<GalileoTrackerConnectionHandler> logger,
        Func<TrackerTypeEnum, IProtocolMessageHandler> handlerProvider)
    {
        _logger = logger;
        _handler = handlerProvider(TrackerTypeEnum.GalileoSkyV50);
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        _logger.LogInformation("{Id} connected", connection.ConnectionId);

        if (connection.RemoteEndPoint is IPEndPoint ipEndPoint)
        { 
            _logger.LogInformation("Получен запрос от {Address}:{Port}", ipEndPoint.Address, ipEndPoint.Port);
        }
            
        List<byte> message = new();
        while (true)
        {
            var result = await connection.Transport.Input.ReadAsync();
            var buffer = result.Buffer;

            foreach (var segment in buffer)
            {
                message.AddRange(segment.ToArray());
            }

            int length = _handler.GetPacketLength(message.ToArray());
            _logger.LogDebug("current length = {Len} for {Id}", length, connection.ConnectionId);
            if (message.Count >= length)
            // больше чем length быть не должно, так как трекер отослав пакет ожидает ответ
            // и только потом шлёт новые данные
            {
                _logger.LogDebug("message.count = {MessageCount} length = {Length} for {Id}", message.Count, length, connection.ConnectionId);
                var resp = _handler.HandleMessage(message.ToArray());
                await connection.Transport.Output.WriteAsync(resp);
                message = new();
            }

            if (result.IsCompleted)
            {
                _logger.LogDebug("result.IsCompleted for {Id}", connection.ConnectionId);
                break;
            }
            connection.Transport.Input.AdvanceTo(buffer.End);
        }

        _logger.LogInformation("{Id} disconnected", connection.ConnectionId);
    }
}