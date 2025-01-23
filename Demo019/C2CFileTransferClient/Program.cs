using TouchSocket.Core;
using TouchSocket.Dmtp.FileTransfer;
using TouchSocket.Dmtp;
using TouchSocket.Rpc;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Sockets;

namespace C2CFileTransferClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("请输入id:");
            var id = Console.ReadLine();
            var client = new TcpDmtpClient();
            await client.SetupAsync(new TouchSocketConfig()
                 .ConfigureContainer(a =>
                 {
                     a.AddConsoleLogger();
                     a.AddRpcStore(store =>
                     {
                         store.RegisterServer<MyClientRpcServer>();
                     });
                     a.AddDmtpRouteService();//添加路由策略
                 })
                 .ConfigurePlugins(a =>
                 {
                     a.UseDmtpFileTransfer();//必须添加文件传输插件
                     a.UseDmtpRpc();
                     a.UseDmtpHeartbeat()
                     .SetTick(TimeSpan.FromSeconds(3))
                     .SetMaxFailCount(3);
                     a.Add<MyFileTransferPermitionPlugin>();
                     a.Add<MyFileTransferRoutPermitPlugin>();
                 })
                 .SetRemoteIPHost("127.0.0.1:8808")
                 .SetDmtpOption(new DmtpOption()
                 {
                     VerifyToken = "Dmtp",
                     Id = id
                 }));
            await client.ConnectAsync();
            client.Logger.Info($"连接成功，Id={client.Id}");
            bool loginResult = (bool)client.GetDmtpRpcActor().Invoke("Login", typeof(bool), InvokeOption.WaitInvoke, "123", "abc");
            Console.WriteLine(loginResult);


            while (true)
            {
                Console.WriteLine("请输入文件传输目标id:");
                var str = Console.ReadLine();
                if (str == "") break;
                var arr = str.Split(' ');
                if (arr.Length == 1)
                {
                    var targetId = arr[0];
                    var filePath = "d:\\百通-440x297-300冰白-双面彩色打印-6份.pdf";
                    var saveFilePath = "c:\\out\\百通-440x297-300冰白-双面彩色打印-6份.pdf";
                    var metadata = new Metadata();//传递到服务器的元数据
                    metadata.Add("1", "1");
                    metadata.Add("2", "2");
                    var fileOperator = new FileOperator()//实例化本次传输的控制器，用于获取传输进度、速度、状态等。
                    {
                        SavePath = saveFilePath,//客户端本地保存路径
                        ResourcePath = filePath,//请求文件的资源路径
                        Metadata = metadata,//传递到服务器的元数据
                        Timeout = TimeSpan.FromSeconds(60),//传输超时时长
                        TryCount = 10,//当遇到失败时，尝试次数
                        FileSectionSize = 1024 * 512,//分包大小，当网络较差时，应该适当减小该值
                    };
                    //此处的作用相当于Timer，定时每秒输出当前的传输进度和速度。
                    var loopAction = LoopAction.CreateLoopAction(-1, 1000, (loop) =>
                    {
                        if (fileOperator.Result.ResultCode != ResultCode.Default)
                        {
                            loop.Dispose();
                        }
                        client.Logger.Info($"进度：{fileOperator.Progress}，速度：{fileOperator.Speed()}");
                    });

                    loopAction.RunAsync();
                    //此方法会阻塞，直到传输结束，也可以使用PushFileAsync
                    IResult result = client.GetDmtpFileTransferActor().PushFile(targetId, fileOperator);
                    Console.WriteLine(result.Message);
                }
            }
            Console.ReadLine();
        }
    }
}
