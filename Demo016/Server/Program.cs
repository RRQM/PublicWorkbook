using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Dmtp;
using TouchSocket.Rpc;
using TouchSocket.Sockets;
using System.Threading;
using TouchSocket.Dmtp.RouterPackage;

namespace Server
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
                Console.Write("Bridge Id:");
                var bridgeId = Console.ReadLine();
                Console.Write("Target Id:");
                var targetId = Console.ReadLine();
                if (service.TryGetSocketClient(bridgeId, out var client))
                {
                    Console.WriteLine("调用RpcA");
                    Invoke(client, targetId, "RpcA", "WWW");
                    //Console.WriteLine("调用RpcB");
                    //Invoke<int>(client, bridgeId, targetId, "RpcB", 1, 2);
                    //Console.WriteLine("调用RpcC");
                    //Invoke<object>(client, bridgeId, targetId, "RpcB", new ClassTest { Name = "GPT", Age = 20 });
                }
                else
                {
                    Console.WriteLine("ID获取失败");
                }
            }
        }

        static void Invoke(HttpDmtpSocketClient client, string targetId, string serviceName, params object[] parameters) 
        {
            //return client.GetDmtpRpcActor().InvokeT<T>(bridgeId, "Bridge", InvokeOption.WaitInvoke, serviceName, targetId, args);
            
            using (var byteBlock = new ByteBlock(1024 * 512))
            {
                var requestPackage = new MyRequestPackage()
                {
                    SourceId = client.Id,
                    TargetId = targetId,
                    SerializationType = SerializationType.FastBinary,
                    Feedback = FeedbackType.OnlySend,
                    MethodName = serviceName,
                    ParametersBytes = new List<byte[]>(),
                    Metadata = new Metadata()
                };
                foreach (var parameter in parameters)
                {
                    requestPackage.ParametersBytes.Add(SerializeConvert.FastBinarySerialize(parameter));
                }

                //发起请求，然后等待一个自定义的响应包。
                var response = client.GetDmtpRouterPackageActor().Request(requestPackage);
                client.Logger.Info($"自定义响应成功，{response}");
            }
        }

        [Serializable]
        public class ClassTest
        {
            public string Name { get; set; }
            public int Age { get; set; }
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
                       a.UseDmtpRouterPackage();
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
