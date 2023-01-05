using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTonFMS.Infrastructure.MessageBus
{
    public interface IMessageHandler
    {
        Task HandleAsync(byte[] message);
    }
}
