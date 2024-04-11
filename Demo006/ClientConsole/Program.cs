using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace ClientConsole
{
    class Program
    {
        static TcpDmtpClient client;
        // static  void Main(string[] args)
        public static void Main(string[] args)
        { 
            CreateClinetAsync();
            Console.ReadKey();

        }
        public static void CreateClinetAsync()
        {
            if ((client is null) == false && client.Online)
            {
                client.Logger.Info($"服务器 {client.IP} 已经连接");
            }
            else
            {
                client = new TcpDmtpClient();
                client.Setup(new TouchSocketConfig()
                       .SetRemoteIPHost(new IPHost($"127.0.0.1:7789"))
                       .ConfigureContainer(a =>
                       {
                           a.AddConsoleLogger();
                           a.AddRpcStore(store =>
                           {
                           store.RegisterServer<ReverseCallbackServer>();//注册服务
                           });

                       })
                       .ConfigurePlugins(a =>
                       {
                           a.UseDmtpRpc(); 
                       }
                       ).SetDmtpOption(new DmtpOption()
                       {
                           VerifyToken = "TouchRpc"
                       }));

                client.Connect();
            }
        }


        public async static Task<bool> PutClientInfoToServerListAsync(string clientId, string macAddress, string computerName)
        {
            var dmtpInvokeOption = new DmtpInvokeOption()//调用配置
            {
                FeedbackType = FeedbackType.WaitInvoke,//调用反馈类型 
                Timeout = 3000 //调用超时设置          
            }; 
            return await client.GetDmtpRpcActor().InvokeTAsync<bool>("FormClientInfoToServerListAsync", dmtpInvokeOption, client.Id, macAddress, computerName);   
        }

    }
}
