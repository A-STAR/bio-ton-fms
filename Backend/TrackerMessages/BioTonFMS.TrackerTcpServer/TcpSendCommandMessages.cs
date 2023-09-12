using BioTonFMS.Domain.Messaging;
using System.Net;

namespace BioTonFMS.TrackerTcpServer;

public class TcpSendCommandMessages
{
    private readonly Dictionary<(string IpAddress, int Port), Queue<TrackerCommandMessage>>  _messages = new Dictionary<(string IpAddress, int Port), Queue<TrackerCommandMessage>>();

    public virtual void AddSendCommandMessage(TrackerCommandMessage message) 
    {
        Queue<TrackerCommandMessage> queue;
        if (_messages.ContainsKey((message.IpAddress, message.Port))) 
        {
            queue = _messages[(message.IpAddress, message.Port)];
        }
        else
        {
            queue = new Queue<TrackerCommandMessage>();
            _messages.Add((message.IpAddress, message.Port), queue);
        }
        queue.Enqueue(message);
    }

    public virtual TrackerCommandMessage? GetCommandForTracker(IPAddress ip, int port)
    {
        TrackerCommandMessage? command = null;
        if (_messages.ContainsKey((ip.ToString(), port)))
        {
            var queue = _messages[(ip.ToString(), port)];
            if (queue.Count > 0)
            {
                command = queue.Dequeue();
            }
        }
        return command;
    }
}
