using Microsoft.Extensions.Logging;
using System;

namespace BioTonFMS.Infrastructure.MessageBus
{
    public interface IMessageBus
    {
        void Publish(byte[] message);

        void Subscribe<TEvenArgs>(EventHandler<TEvenArgs> handler);
    }
}
