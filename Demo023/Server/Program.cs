using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Server
{
    internal class Program
    {
        private static ManualResetEvent _manualReset = new(false);
        static async Task Main(string[] args)
        {
            var service = new HttpService();

            await service.SetupAsync(new TouchSocketConfig()
                 .SetListenIPHosts(9010)
                 .ConfigurePlugins(a =>
                 {
                     a.UseWebSocket()
                     .SetWSUrl("/ws");
                     a.UseWebSocketJsonRpc()
                     .SetAllowJsonRpc((SessionClient, context) =>
                     {
                         return true;
                     });
                     a.Add<MyPluginClass>();
                 }));

            await service.StartAsync();
            _manualReset.WaitOne();
        }
    }
}
