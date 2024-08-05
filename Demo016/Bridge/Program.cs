using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.RouterPackage;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.NamedPipe;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Bridge
{
    internal class Program
    {
        public static NamedPipeDmtpService pipeService { get; set; }
        public static HttpDmtpClient httpDmtpClient { get; set; }
        static string clientId { get; set; }

        static long m_idCounter;

        public partial class MyRpcServer : RpcServer
        {
            [DmtpRpc(true)]
            public void Bridge(string service, string targetId, string args)
            {
                //if (pipeService.TryGetSocketClient(targetId, out var client))
                //{
                //    client.GetDmtpRpcActor().Invoke(service, InvokeOption.WaitInvoke, args);
                //    Console.WriteLine("调用成功");
                //}
            }
        }

        static void Main(string[] args)
        {

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
                .ConfigureContainer(a =>
                {
                    a.AddRpcStore(store =>
                    {
                        store.RegisterServer<MyRpcServer>();
                    });
                })
                .ConfigurePlugins(a =>
                {
                    a.UseDmtpRpc();
                    a.UseDmtpRouterPackage();
                    a.Add<MyPlugin>();
                })
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Dmtp",
                }));

            return client;
        }

        static NamedPipeDmtpService CreateNamedPipeDmtpService(string pipeName)
        {
            var service = new NamedPipeDmtpService();
            var config = new TouchSocketConfig()//配置
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
}
