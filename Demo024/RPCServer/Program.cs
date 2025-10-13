using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace RPCServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var service = new TcpDmtpService();
            var config = new TouchSocketConfig()//配置
                   .SetListenIPHosts(49999)
                   .ConfigureContainer(a =>
                   {
                       a.AddConsoleLogger();

                       a.AddRpcStore(store =>
                       {
                           store.RegisterServer<RPC>();
                       });
                   })
                   .ConfigurePlugins(a =>
                   {
                       a.UseDmtpRpc();//启用DmtpRpc功能
                   })
            #region Dmtp服务器基础配置
                   .SetDmtpOption(options =>
                   {
                       options.VerifyToken = "Dmtp";//设定连接口令，作用类似账号密码
                       options.VerifyTimeout = TimeSpan.FromSeconds(3);//设定账号密码验证超时时间
                   })
            #endregion
                   ;

            await service.SetupAsync(config);
            await service.StartAsync();
            Console.ReadLine();
        }
    }
}
