using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace TcpClient
{
    internal class Program
    {
        static string id { get; set; }
        public partial class MyRpcServer : RpcServer
        {
            [DmtpRpc(true)]
            public void SayHello(string name)
            {
                Console.WriteLine($"{name},hi {id}");
            }
        }
        static void Main(string[] args)
        {
            id = args[0];
            var client = new TcpDmtpClient();
            client.Setup(new TouchSocketConfig()
                .SetRemoteIPHost("127.0.0.1:8001")
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
            client.Connect();
            Result res = client.TryConnect();
            Console.WriteLine($"Connection {res.Message} - ID: {id}");
            while (true)
            {
                string targetId = Console.ReadLine();
                client.GetDmtpRpcActor().Invoke(targetId, "SayHello", InvokeOption.WaitInvoke, "李四");
            }
        }
    }
}
