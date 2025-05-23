using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Rpc;

namespace Server
{
    internal class DmtpHandshakedPlugin : PluginBase, IDmtpHandshakedPlugin
    {
        private readonly ILogger<DmtpHandshakedPlugin> m_logger;
        public DmtpHandshakedPlugin(ILogger<DmtpHandshakedPlugin> logger)
        {
            this.m_logger = logger;
        }
        public async Task OnDmtpHandshaked(IDmtpActorObject client, DmtpVerifyEventArgs e)
        {
            if (e.Metadata.TryGetValue("Role", out var role) &&
                e.Metadata.TryGetValue("Type", out var clientType))
            {
                string newId = $"{role}:{clientType}:{Guid.NewGuid()}";
                await client.DmtpActor.ResetIdAsync(newId);
            }
            this.m_logger.LogInformation("Client handshaked,Id={0}", client.DmtpActor.Id);
            await e.InvokeNext();
        }
    }
}
