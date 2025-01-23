using TouchSocket.Core;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.FileTransfer;
using TouchSocket.Sockets;
using TouchSocket.Rpc;
using C2CFileTransferClient;

namespace C2CFileTransferServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var service = new TcpDmtpService();
            var config = new TouchSocketConfig()//配置
                   .SetListenIPHosts(8808)
                   .ConfigureContainer(a =>
                   {
                       a.AddConsoleLogger();
                       a.AddDmtpRouteService();//添加路由策略
                       a.AddRpcStore(store =>
                       {
                           store.RegisterServer<MyRpcServer>();
                       });
                   })
                   .ConfigurePlugins(a =>
                   {
                       a.UseDmtpFileTransfer();//必须添加文件传输插件
                       a.UseDmtpRpc();
                       
                       a.Add<MyFileTransferRoutPermitPlugin>();
                   })
                   .SetDmtpOption(new DmtpOption()
                   {
                       VerifyToken = "Dmtp"//设定连接口令，作用类似账号密码
                   });

            await service.SetupAsync(config);

            await service.StartAsync();

            service.Logger.Info($"{service.GetType().Name}已启动");

            service.Logger.Info($"输入客户端Id，空格输入消息，将通知客户端方法");
            while (true)
            {
                var str = Console.ReadLine();
                if (service.TryGetClient(str.Split(' ')[0], out var socketClient))
                {
                    var result =await socketClient.GetDmtpRpcActor().InvokeTAsync<bool>("Notice", DmtpInvokeOption.WaitInvoke, str.Split(' ')[1]);
                    service.Logger.Info($"调用结果{result}");
                }
            }
        }
    }
}
