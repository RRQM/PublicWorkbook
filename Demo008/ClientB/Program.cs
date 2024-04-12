using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.NamedPipe;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace ClientB
{
    internal class Program
    {
        static NamedPipeDmtpService pipeService { get; set; }
        static HttpDmtpClient httpDmtpClient { get; set; }
        static string clientId { get; set; }

        static long m_idCounter;

        public partial class MyRpcServer : RpcServer
        {
            [DmtpRpc(true)]
            public void Bridge(string targetId, string method, string args)
            {
                if (pipeService.TryGetSocketClient(targetId, out var client))
                {
                    client.GetDmtpRpcActor().Invoke(method, InvokeOption.WaitInvoke, args);
                    Console.WriteLine("调用成功");
                }
            }
        }

        static Container m_container = new Container();

        static void Main(string[] args)
        {
            //issue https://gitee.com/RRQM_Home/TouchSocket/issues/I9G1GD

            #region 企业版测试
            try
            {
                Enterprise.ForTest();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            #endregion

            pipeService = CreateNamedPipeDmtpService("TouchSocketPipe");
            pipeService.Start();
            httpDmtpClient = CreateHttpDmtpClient(7789);
            var res = httpDmtpClient.TryConnect();
            clientId = httpDmtpClient.Id;
            Console.WriteLine($"Connection {res.Message} - ID: {clientId}");
            while (true)
            {
                Console.ReadKey();
            }
        }

        static HttpDmtpClient CreateHttpDmtpClient(int port)
        {
            var client = new HttpDmtpClient();
            client.Setup(new TouchSocketConfig()
                .SetRemoteIPHost($"127.0.0.1:{port}")
                .SetRegistrator(m_container)
                .ConfigureContainer(a =>
                {
                    a.AddRpcStore(store =>
                    {
                        store.RegisterServer<MyRpcServer>();
                    });

                    a.RegisterSingleton<IDmtpRouteService, MyDmtpRouteService>();
                })
                .ConfigurePlugins(a =>
                {
                    a.UseDmtpRpc();
                })
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Dmtp"
                }));

            return client;
        }

        static NamedPipeDmtpService CreateNamedPipeDmtpService(string pipeName)
        {
            var service = new NamedPipeDmtpService();
            var config = new TouchSocketConfig()//配置
                   .SetRegistrator(m_container)
                   .SetPipeName(pipeName)//设置管道名称
                   .SetGetDefaultNewId(() => $"PipeDmtp-{Interlocked.Increment(ref m_idCounter)}")
                   .ConfigureContainer(a =>
                   {
                       a.RegisterSingleton<INamedPipeDmtpService, NamedPipeDmtpService>();

                       a.AddConsoleLogger();
                   })
                   .ConfigurePlugins(a =>
                   {
                       a.UseDmtpRpc();
                   })
                   .SetDmtpOption(new DmtpOption()
                   {
                       VerifyToken = "Dmtp"//设定连接口令，作用类似账号密码
                   });

            service.Setup(config);
            return service;
        }
    }

    class MyDmtpRouteService : IDmtpRouteService
    {
        private readonly INamedPipeDmtpService m_namedPipeDmtpService;

        public Func<string, Task<IDmtpActor>> FindDmtpActor { get ; set; }

        public MyDmtpRouteService(INamedPipeDmtpService namedPipeDmtpService)
        {
            this.m_namedPipeDmtpService = namedPipeDmtpService;
            this.FindDmtpActor = this.OnFindDmtpActor;
        }

        private async Task<IDmtpActor> OnFindDmtpActor(string id)
        {
            await Task.CompletedTask;

            if (m_namedPipeDmtpService.TryGetSocketClient(id,out var socketClient))
            {
                return socketClient.DmtpActor;
            }

            return default;
        }
    }
}
