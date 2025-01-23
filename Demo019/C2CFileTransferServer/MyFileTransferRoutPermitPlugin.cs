using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;

namespace C2CFileTransferClient
{
    internal class MyFileTransferRoutPermitPlugin : PluginBase, IDmtpRoutingPlugin
    {
        public async Task OnDmtpRouting(IDmtpActorObject client, PackageRouterEventArgs e)
        {
            Console.WriteLine(e.RouterType);
            e.IsPermitOperation = true;//允许路由
            await e.InvokeNext();
        }
    }
}
