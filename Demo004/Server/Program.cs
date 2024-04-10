using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateTcpDmtpService(8001);
            CreateHttpDmtpService(8002);
            while (true)
            {

            }
        }
        
        static void CreateTcpDmtpService(int port)
        {
            var service = new TcpDmtpService();
            var config = new TouchSocketConfig()//配置
                   .SetListenIPHosts(port)
                   .ConfigureContainer(a =>
                   {
                       a.AddConsoleLogger();
                       a.AddDmtpRouteService();
                   })
                   .ConfigurePlugins(a =>
                   {
                       a.UseDmtpRpc();
                       a.Add<MyPlugin>();
                   })
                   .SetDmtpOption(new DmtpOption()
                   {
                       VerifyToken = "Dmtp"//设定连接口令，作用类似账号密码
                   });

            service.Setup(config);

            service.Start();

            service.Logger.Info($"Tcp[{service.GetType().Name}]已启动");
        }

        static void CreateHttpDmtpService(int port)
        {
            var service = new HttpDmtpService();
            var config = new TouchSocketConfig()//配置
                   .SetListenIPHosts(port)
                   .ConfigureContainer(a =>
                   {
                       a.AddConsoleLogger();
                       a.AddDmtpRouteService();
                   })
                   .ConfigurePlugins(a =>
                   {
                       a.UseDmtpRpc();
                       a.Add<MyPlugin>();
                   })
                   .SetDmtpOption(new DmtpOption()
                   {
                       VerifyToken = "Dmtp"//设定连接口令，作用类似账号密码
                   });

            service.Setup(config);

            service.Start();

            service.Logger.Info($"Http[{service.GetType().Name}] 已启动");
        }

        internal class MyPlugin : PluginBase, IDmtpRoutingPlugin
        {
            public async Task OnDmtpRouting(IDmtpActorObject client, PackageRouterEventArgs e)
            {
                if (e.RouterType == RouteType.Rpc)
                {
                    e.IsPermitOperation = true;
                    return;
                }

                await e.InvokeNext();
            }
        }
    }
}
