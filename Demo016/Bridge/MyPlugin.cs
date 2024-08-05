using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.RouterPackage;
using TouchSocket.Dmtp.Rpc;

namespace Bridge
{
    internal class MyPlugin : PluginBase, IDmtpRouterPackagePlugin
    {
        private readonly ILog m_logger;
        public MyPlugin(ILog logger)
        {
            this.m_logger = logger;
        }

        public async Task OnReceivedRouterPackage(IDmtpActorObject client, RouterPackageEventArgs e)
        {
            var response = e.ReadRouterPackage<MyRequestPackage>();
            var byteBlock = new ByteBlock();
            response.Package(byteBlock);


            if (Program.pipeService.TryGetSocketClient(response.TargetId, out var socketClient))
            {
                socketClient.GetDmtpRpcActor().DmtpActor.Send(20, byteBlock);
            }
            await e.InvokeNext();
        }


    }
}
