using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.NamedPipe;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Client
{
    internal class Program
    {
        private static string id { get; set; }
        public partial class MyRpcServer : RpcServer
        {
            [DmtpRpc(true)]
            public void RpcA(string name)
            {
                Console.WriteLine($"{name}: hi {id}");
            }

            [DmtpRpc(true)]
            public int RpcB(int a, int b)
            {
                return a + b;
            }

            [DmtpRpc(true)]
            public void RpcC(ClassTest classTest)
            {
                Console.WriteLine($"Name: {classTest.Name}\t Age: {classTest.Age}");
            }
        }

        [Serializable]
        public class ClassTest
        {
            public string Name { get; set; }
            public int Age { get; set; }
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

            var client = CreateNamedPipeDmtpClient("TouchSocketPipe");
            var res = client.TryConnect();
            string clientId = client.Id;
            Console.WriteLine($"Connection {res.Message} - ID: {clientId}");
            while (true)
            {
                Console.ReadKey();
            }
        }

        static NamedPipeDmtpClient CreateNamedPipeDmtpClient(string pipeName)
        {
            var client = new NamedPipeDmtpClient();
            client.Setup(new TouchSocketConfig()
                .SetPipeName(pipeName)
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
                    VerifyToken = "Dmtp"
                }));

            return client;
        }
    }
}
