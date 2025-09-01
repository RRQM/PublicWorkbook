using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Http.WebSockets;
using TouchSocket.JsonRpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Server
{
    internal class MyPluginClass : PluginBase, IWebSocketHandshakedPlugin, IWebSocketHandshakingPlugin, IWebSocketClosedPlugin
    {
        public async Task OnWebSocketClosed(IWebSocket webSocket, ClosedEventArgs e)
        {
            Console.WriteLine("断开连接");
            await e.InvokeNext();
        }

        public async Task OnWebSocketHandshaked(IWebSocket webSocket, HttpContextEventArgs e)
        {
            Console.WriteLine("握手成功");
            await e.InvokeNext();
        }

        public async Task OnWebSocketHandshaking(IWebSocket webSocket, HttpContextEventArgs e)
        {
            Console.WriteLine("握手中..");
            await e.InvokeNext();
        }
    }
}
