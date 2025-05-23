using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Server
{
    internal class DmtpClosedPlugin : PluginBase, IDmtpClosedPlugin
    {
        private readonly ILogger<DmtpClosedPlugin> m_logger;

        public DmtpClosedPlugin(ILogger<DmtpClosedPlugin> logger)
        {
            this.m_logger = logger;
        }

        public async Task OnDmtpClosed(IDmtpActorObject client, ClosedEventArgs e)
        {
            var id = client.DmtpActor.Id;
            this.m_logger.LogInformation("Client disconnected,Id={0}", client.DmtpActor.Id);
            await e.InvokeNext();
        }
    }

}
