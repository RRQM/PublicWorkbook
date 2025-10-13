using TouchSocket.Rpc;
using TouchSocket.Rpc.TouchRpc;
using TouchSocket.Sockets;

namespace RPCClientOld
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            TouchSocket.Core.TouchSocketConfig rpc_config = new TouchSocket.Core.TouchSocketConfig();
            rpc_config.SetRemoteIPHost(new IPHost($"127.0.0.1:49999"));

            var tcpclient_rpc = new TouchSocket.Rpc.TouchRpc.TcpTouchRpcClient();
            tcpclient_rpc.Setup(rpc_config);
            tcpclient_rpc.Connect();
            IInvokeOption invokeOption = default;
            System.Int32 returnData = tcpclient_rpc.Invoke<System.Int32>("RPCServer.RPC.Test".ToLower(), invokeOption, 1,2);
            Console.WriteLine(returnData);
        }
    }
}
