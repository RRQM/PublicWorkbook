using System;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace HttpClient
{
    internal class Program
    {
        private static string id { get; set; }
        public partial class MyRpcServer : RpcServer
        {
            [DmtpRpc(true)]
            public void SayHello(string name)
            {
                Console.WriteLine($"{name},hi {id}");
            }
        }

        private static void Main(string[] args)
        {
            var client = new HttpDmtpClient();
            client.Setup(new TouchSocketConfig()
                .SetRemoteIPHost("127.0.0.1:8002")
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
                })
                .SetDmtpOption(new DmtpOption()
                {
                    Id = id,
                    VerifyToken = "Dmtp"
                }));

            var res = client.TryConnect();

            id = client.Id;

            Console.WriteLine($"Connection {res.Message} - ID: {id}");
            while (true)
            {
                var targetId = Console.ReadLine();
                client.GetDmtpRpcActor().Invoke(targetId, "SayHello", InvokeOption.WaitInvoke, "Http协议");
                Console.WriteLine("调用成功");
            }
        }
    }
}
