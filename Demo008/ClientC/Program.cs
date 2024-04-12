using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.NamedPipe;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace ClientC
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
