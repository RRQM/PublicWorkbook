using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace RPCUpdateTest
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            if (true)
            {
                var Client = new TouchSocket.Dmtp.TcpDmtpClient();
                var x = new TouchSocketConfig().SetRemoteIPHost("127.0.0.1:49999").ConfigurePlugins(a =>
                {
                     a.UseDmtpRpc();
                });
                x.SetDmtpOption(new Action<DmtpOption>((a) =>
                {
                    a.VerifyToken = "Dmtp";
                  
                }));
                await Client.SetupAsync(x);

                await Client.ConnectAsync();

                System.Int32 returnData = await Client.GetDmtpRpcActor().InvokeTAsync<System.Int32>("RPCServer.RPC.Test".ToLower(), InvokeOption.WaitInvoke, 1, 2);
                Console.WriteLine(returnData);
            }
            if (false)
            {
                var Client = new TouchSocket.WebApi.WebApiClient();
                await Client.SetupAsync(new TouchSocket.Core.TouchSocketConfig().SetRemoteIPHost(new IPHost($"127.0.0.1:49999")));
                await Client.ConnectAsync();
                TouchSocket.Rpc.InvokeOption invokeOption = new InvokeOption()
                {
                    FeedbackType = TouchSocket.Rpc.FeedbackType.WaitInvoke,
                    Timeout = 5000,

                };
                System.Int32 returnData = await Client.InvokeTAsync<System.Int32>("RPCServer.RPC.Test".ToLower(), invokeOption, 1, 2);
                Console.WriteLine(returnData);
            }
            if (true)
            {
                var Client = new TouchSocket.JsonRpc.TcpJsonRpcClient();
                await Client.SetupAsync(new TouchSocket.Core.TouchSocketConfig().
                    SetRemoteIPHost(new IPHost($"127.0.0.1:49999"))
                   .SetTcpDataHandlingAdapter(() => new JsonPackageAdapter(Encoding.UTF8)));
                await Client.ConnectAsync();
                System.Int32 returnData = await Client.InvokeTAsync<System.Int32>
                    ("RPCServer.RPC.Test".ToLower(), InvokeOption.WaitInvoke,new object[] { 1, 2 });
                Console.WriteLine(returnData);
            }

            if (true)
            {
                var Client = new TouchSocket.XmlRpc.XmlRpcClient();
                await Client.SetupAsync(new TouchSocket.Core.TouchSocketConfig().
                    SetRemoteIPHost(new IPHost($"127.0.0.1:49999")));
                await Client.ConnectAsync();
                System.Int32 returnData = await Client.InvokeTAsync<System.Int32>("RPCServer.RPC.Test".ToLower(), InvokeOption.WaitInvoke, 1, 2);
                Console.WriteLine(returnData);
            }
            if (true)
            {
                var Client = new TouchSocket.XmlRpc.XmlRpcClient();
                await Client.SetupAsync(new TouchSocket.Core.TouchSocketConfig().
                    SetRemoteIPHost(new IPHost($"127.0.0.1:49999")));
                await Client.ConnectAsync();
                System.Int32 returnData = await Client.InvokeTAsync<System.Int32>("RPCServer.RPC.Test".ToLower(), InvokeOption.WaitInvoke, 1, 2);
                Console.WriteLine(returnData);
            }
        }
    }
}
