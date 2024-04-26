using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.FileTransfer;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Server
{
    internal class Program
    {
        public static IHost host { get; set; }
        private static void Main(string[] args)
        {
            host = Host.CreateDefaultBuilder(args)
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
                        a.AddRpcStore(store =>
                        {
                            store.RegisterServer<MyRpcServer>();
                        });
                        a.RegisterSingleton<IDmtpRouteService, MyDmtpRouteService>();
                    });

                    services.AddServiceHostedService<ITcpDmtpService, TcpDmtpService>(config =>
                    {
                        config.SetListenIPHosts(8001)
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

    public partial class MyRpcServer : RpcServer
    {
        [DmtpRpc(true)]
        public string[] GetIds(string type)
        {
            using var scope = Program.host.Services.CreateScope();
            var services = scope.ServiceProvider;
            if (type.Contains("HttpDmtp"))
            {
                var service = services.GetRequiredService<IHttpDmtpService>();
                return service.GetIds().ToArray();
            }
            else
            {
                var service = services.GetRequiredService<ITcpDmtpService>();
                return service.GetIds().ToArray();
            }
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

            if (id.StartsWith("TcpDmtp") || id.StartsWith("Transfer-TcpDmtp"))
            {
                if (m_tcpDmtpService.TryGetSocketClient(id,out var socketClient))
                {
                   return socketClient.DmtpActor;
                }
            }
            else if (id.StartsWith("HttpDmtp") || id.StartsWith("Transfer-TcpDmtp"))
            {
                if (m_httpDmtpService.TryGetSocketClient(id, out var socketClient))
                {
                    return socketClient.DmtpActor;
                }
            }

            return null;
        }
    }

    internal class MyPlugin : PluginBase, IDmtpFileTransferingPlugin, IDmtpFileTransferedPlugin, IDmtpRoutingPlugin
    {
        private readonly ILog m_logger;

        public MyPlugin(ILog logger)
        {
            this.m_logger = logger;
        }

        /// <summary>
        /// 该方法，会在每个文件被请求（推送）结束时触发。传输不一定成功，具体信息需要从e.Result判断状态。
        /// 其次，该方法也不一定会被执行，例如：在传输过程中，直接断网，则该方法将不会执行。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task OnDmtpFileTransfered(IDmtpActorObject client, FileTransferedEventArgs e)
        {
            //传输结束，但是不一定成功，甚至该方法都不一定会被触发，具体信息需要从e.Result判断状态。
            if (e.TransferType.IsPull())
            {
                this.m_logger.Info($"结束Pull文件，类型={e.TransferType}，文件名={e.ResourcePath}，结果={e.Result}");
            }
            else
            {
                this.m_logger.Info($"结束Push文件，类型={e.TransferType}，文件名={e.FileInfo.Name}，结果={e.Result}");
            }
            await e.InvokeNext();
        }

        /// <summary>
        /// 该方法，会在每个文件被请求（推送）时第一时间触发。
        /// 当请求文件时，可以重新指定请求的文件路径，即对e.ResourcePath直接赋值。
        /// 当推送文件时，可以重新指定保存文件路径，即对e.SavePath直接赋值。
        ///
        /// 注意：当文件夹不存在时，需要手动创建。
        /// </summary>
        /// <param name="client"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public async Task OnDmtpFileTransfering(IDmtpActorObject client, FileTransferingEventArgs e)
        {
            foreach (var item in e.Metadata.Keys)
            {
                Console.WriteLine($"Key={item},Value={e.Metadata[item]}");
            }
            e.IsPermitOperation = true;//每次传输都需要设置true，表示允许传输
            //有可能是上传，也有可能是下载

            if (e.TransferType.IsPull())
            {
                this.m_logger.Info($"请求Pull文件，类型={e.TransferType}，文件名={e.ResourcePath}");
            }
            else
            {
                this.m_logger.Info($"请求Push文件，类型={e.TransferType}，文件名={e.FileInfo.Name}");
            }
            await e.InvokeNext();
        }

        public async Task OnDmtpRouting(IDmtpActorObject client, PackageRouterEventArgs e)
        {
            e.IsPermitOperation = true;//允许路由
            this.m_logger.Info($"路由类型：{e.RouterType}");

            await e.InvokeNext();
        }
    }
}
