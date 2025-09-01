using TouchSocket.Core;
using TouchSocket.JsonRpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Demo23
{
    internal class Program
    {
        private static ManualResetEvent _manualReset = new(false);
        static async Task Main(string[] args)
        {
            var client = new WebSocketJsonRpcClient();
            await client.SetupAsync(new TouchSocketConfig()
                .SetRemoteIPHost("ws://127.0.0.1:9010/ws")
                .ConfigurePlugins(a =>
                {
                    a.UseWebSocketHeartbeat()
                    .SetTick(TimeSpan.FromSeconds(5));
                    a.UseWebSocketReconnection()
                    .UsePolling(TimeSpan.FromSeconds(5));
                }));
            var result = await client.TryConnectAsync();
            Console.WriteLine(result);
            _manualReset.WaitOne();
        }
    }
}
