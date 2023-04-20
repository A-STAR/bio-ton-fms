﻿using BioTonFMS.Domain;
using BioTonFMS.TrackerTcpServer.ProtocolMessageHandlers;
using Microsoft.AspNetCore.Connections;
using System.Net;

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
            if (length > 0)
            {
                _logger.LogDebug("current length = {Len} for {Id}", length, connection.ConnectionId);
                if (message.Count >= length)
                // больше чем length быть не должно, так как трекер отослав пакет ожидает ответ
                // и только потом шлёт новые данные
                {
                    _logger.LogDebug("message.count = {MessageCount} length = {Length} for {Id}", message.Count, length,
                        connection.ConnectionId);

                    byte[] resp;
                    if (connection.RemoteEndPoint is IPEndPoint endpoint)
                    {
                        resp = _handler.HandleMessage(message.ToArray(), endpoint.Address, endpoint.Port);
                    }
                    else
                    {
                        resp = _handler.HandleMessage(message.ToArray(), ip: IPAddress.None, port: 0);
                    }

                    await connection.Transport.Output.WriteAsync(resp);
                    message = new();
                }
            }
            else
            {
                _logger.LogDebug("Возможная проблема с сообщением {Message}, продолжаем приём данных для {Id}",
                    string.Join(' ', message.Select(x => x.ToString("X"))), connection.ConnectionId);
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