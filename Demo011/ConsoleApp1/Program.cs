using TouchSocket.Core;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // issue https://gitee.com/RRQM_Home/TouchSocket/issues/I9GG05
            var client = new WebSocketClient();
            client.Setup(new TouchSocketConfig()
                .SetRemoteIPHost("ws://127.0.0.1:8888")
                .ConfigureContainer(a =>
                {
                    a.AddConsoleLogger();
                }));
            client.Connect();
            client.Logger.Info("连接成功");

            Console.ReadKey();
        }
    }
}
