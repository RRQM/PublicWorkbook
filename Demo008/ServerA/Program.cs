using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace ServerA
{
    internal class Program
    {
        static long m_idCounter;
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

            var service = CreateHttpDmtpService(7789);
            service.Start();
            service.Logger.Info($"{service.GetType().Name}已启动");
            while (true)
            {
                Console.WriteLine("通过ClientB调用ClientC的DmtpRPC");
                // BridgeClient Id: HttpDmtpClient的Id
                Console.Write("BridgeClient Id:");   
                var clientId = Console.ReadLine();
                // Target Id: NamedPipeDmtpClient的Id，能否不通过Bridge-Rpc只用一个TargetId对NamedPipeDmtpClient进行DmtpRpc调用
                Console.Write("Target Id:");            
                var targetId = Console.ReadLine();
                if (service.TryGetSocketClient(clientId, out var client))
                {
                    client.GetDmtpRpcActor().Invoke(clientId, "Bridge", InvokeOption.WaitInvoke, targetId, "SayHello", "Http协议");
                    Console.WriteLine("调用成功");
                }
            }
        }

        static HttpDmtpService CreateHttpDmtpService(int port)
        {
            var service = new HttpDmtpService();
            var config = new TouchSocketConfig()//配置
                   .SetListenIPHosts(port)
                   .SetGetDefaultNewId(() => $"HttpDmtp-{Interlocked.Increment(ref m_idCounter)}")
                   .ConfigureContainer(a =>
                   {
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
