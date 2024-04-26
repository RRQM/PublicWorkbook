using System;
using System.IO;
using System.Linq;
using System.Threading;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.FileTransfer;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace HttpClient
{
    internal class Program
    {
        static long m_idCounter;
        public const long FileLength = 1024 * 1024 * 10L;

        private static void Main(string[] args)
        {
            #region 企业版测试
            try
            {
                Enterprise.ForTest();
            }
            catch
            {
            }
            #endregion
            Console.Write("Input your main Id: ");
            string mainId = Console.ReadLine();
            using var clientFactory = CreateClientFactory(mainId);
            Result resultCon = clientFactory.CheckStatus();
            Console.WriteLine($"Connect {resultCon.ResultCode}");
            while (true)
            {
                Console.Write("Input target Id: ");
                string targetId = Console.ReadLine();
                string[] ids = clientFactory.MainClient.GetDmtpRpcActor().InvokeT<string[]>("GetIds", InvokeOption.WaitInvoke, targetId);
                string[] targetIds = ids.Where(id => id.Contains(targetId)).ToArray();
                MultithreadingClientPushFileFromService(clientFactory, targetId, targetIds);
            }
        }

        private static HttpDmtpClientFactory CreateClientFactory(string mainId)
        {
            try
            {
                Enterprise.ForTest();
            }
            catch (Exception ex)
            {
                ConsoleLogger.Default.Exception(ex);
            }
            var clientFactory = new HttpDmtpClientFactory()
            {
                MinCount = 5,
                MaxCount = 10,
                OnGetTransferConfig = () => //配置辅助通信
                {
                    return new TouchSocketConfig()
                    .SetRemoteIPHost("127.0.0.1:8002")
                    .SetDmtpOption(new DmtpOption()
                    {
                        VerifyToken = "Dmtp",
                        Id = $"Transfer-HttpDmtp:{mainId}:{Interlocked.Increment(ref m_idCounter)}"
                    })
                    .ConfigurePlugins(a =>
                    {
                        a.UseDmtpFileTransfer();
                    });
                }
            };
            clientFactory.MainConfig//配置主通信
                         .SetRemoteIPHost("127.0.0.1:8002")
                         .SetDmtpOption(new DmtpOption()
                         {
                             VerifyToken = "Dmtp",
                             Id = $"HttpDmtp:{mainId}"
                         })
                         .ConfigurePlugins(a =>
                         {
                             a.UseDmtpRpc();
                             a.UseDmtpFileTransfer();
                         });
            return clientFactory;
        }

        private static void MultithreadingClientPushFileFromService(HttpDmtpClientFactory clientFactory, string targetId, string[] targetIds)
        {
            var resultCon = clientFactory.CheckStatus();//检验连接状态，一般当主通行器连接时，即认为在连接状态。

            if (!resultCon.IsSuccess())
            {
                //没有连接
                return;
            }
            ConsoleLogger.Default.Info("开始推送文件");

            /****此处的逻辑是在程序运行目录下创建一个空内容，但是有长度的文件，用于测试****/
            var filePath = "MultithreadingClientPushFileFromService.Test";
            var saveFilePath = "SaveMultithreadingClientPushFileFromService.Test";

            if (!File.Exists(filePath))//创建服务器端的测试文件
            {
                using (var stream = File.OpenWrite(filePath))
                {
                    stream.SetLength(FileLength);
                }
            }
            /****此处的逻辑是在程序运行目录下创建一个空内容，但是有长度的文件，用于测试****/

            var metadata = new Metadata();//传递到服务器的元数据
            metadata.Add("1", "1");
            metadata.Add("2", "2");

            var fileOperator = new MultithreadingFileOperator//实例化本次传输的控制器，用于获取传输进度、速度、状态等。
            {
                SavePath = saveFilePath,//客户端本地保存路径
                ResourcePath = filePath,//请求文件的资源路径
                Metadata = metadata,//传递到服务器的元数据
                Timeout = TimeSpan.FromSeconds(60),//传输超时时长
                TryCount = 10,//当遇到失败时，尝试次数
                FileSectionSize = 1024 * 512,//分包大小，当网络较差时，应该适当减小该值
                MultithreadingCount = 10//多线程数量
            };

            //此处的作用相当于Timer，定时每秒输出当前的传输进度和速度。
            var loopAction = LoopAction.CreateLoopAction(-1, 1000, (loop) =>
            {
                if (fileOperator.IsEnd)
                {
                    loop.Dispose();
                }
                ConsoleLogger.Default.Info($"进度：{fileOperator.Progress}，速度：{fileOperator.Speed()}");
            });

            loopAction.RunAsync();

            clientFactory.SetFindTransferIds((client, targetId) =>
            {
                //此处的操作不唯一，可能需要rpc实现。
                //其目的比较简单，就是获取到targetId对应的主客户端的所有传输客户端的Id集合。
                //这样就实现了多个客户端向多个客户端传输文件的目的。

                return targetIds;
            });

            //此方法会阻塞，直到传输结束，也可以使用PushFileAsync
            IResult result = clientFactory.PushFile(targetId, fileOperator);

            ConsoleLogger.Default.Info($"推送文件结束，{result}");

            //删除测试文件。此逻辑在实际使用时不要有
            File.Delete(filePath);
            File.Delete(saveFilePath);
        }


    }
}
