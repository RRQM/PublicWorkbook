using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Sockets;

namespace Server
{
    internal class Program
    {
        static long m_idCounter;
        private static void Main(string[] args)
        {
            //https://gitee.com/RRQM_Home/TouchSocket/issues/I9FJGK

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    //Host模式是共用容器，所以最好统一配置
                    services.ConfigureContainer(a =>
                    {
                        a.AddLogger(logger => 
                        {
                            logger.AddConsoleLogger();
                            logger.AddFileLogger();
                        });

                        a.RegisterSingleton<IDmtpRouteService, MyDmtpRouteService>();
                    });

                    services.AddServiceHostedService<ITcpDmtpService, TcpDmtpService>(config =>
                    {
                        config.SetListenIPHosts(8001)
                        .SetGetDefaultNewId(()=>$"TcpDmtp-{Interlocked.Increment(ref m_idCounter)}")
                        .ConfigurePlugins(a =>
                        {
                            a.UseDmtpRpc();
                            a.Add<MyPlugin>();
                        })
                        .SetDmtpOption(new DmtpOption()
                        {
                            VerifyToken = "Dmtp"//设定连接口令，作用类似账号密码
                        });
                    });


                    services.AddServiceHostedService<IHttpDmtpService, HttpDmtpService>(config =>
                    {
                        config.SetListenIPHosts(8002)
                        .SetGetDefaultNewId(() => $"HttpDmtp-{Interlocked.Increment(ref m_idCounter)}")
                        .ConfigurePlugins(a =>
                        {
                            a.UseDmtpRpc();
                            a.Add<MyPlugin>();
                        })
                        .SetDmtpOption(new DmtpOption()
                        {
                            VerifyToken = "Dmtp"//设定连接口令，作用类似账号密码
                        });
                    });
                })
                .Build();

            host.Run();
        }
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

    class MyDmtpRouteService : IDmtpRouteService
    {
        private readonly ITcpDmtpService m_tcpDmtpService;
        private readonly IHttpDmtpService m_httpDmtpService;

        public Func<string, Task<IDmtpActor>> FindDmtpActor { get ; set ; }

        public MyDmtpRouteService(ITcpDmtpService tcpDmtpService,IHttpDmtpService httpDmtpService)
        {
            this.FindDmtpActor = this.OnFindDmtpActor;
            this.m_tcpDmtpService = tcpDmtpService;
            this.m_httpDmtpService = httpDmtpService;
        }

        private async Task<IDmtpActor> OnFindDmtpActor(string id)
        {
            await Task.CompletedTask;

            if (id.StartsWith("TcpDmtp"))
            {
                if (m_tcpDmtpService.TryGetSocketClient(id,out var socketClient))
                {
                   return socketClient.DmtpActor;
                }
            }
            else if (id.StartsWith("HttpDmtp"))
            {
                if (m_httpDmtpService.TryGetSocketClient(id, out var socketClient))
                {
                    return socketClient.DmtpActor;
                }
            }

            return null;
        }
    }
}
