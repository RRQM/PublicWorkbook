using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Http.WebSockets;
using TouchSocket.Sockets;

namespace WSDemo
{
    /// <summary>
    /// WebSocket服务A
    /// </summary>
    public class WebSocketPluginA : PluginBase, IWebSocketReceivedPlugin
    {
        private ILog m_logger;

        public WebSocketPluginA(ILog logger)
        {
            this.m_logger = logger;
        }
        public async Task OnWebSocketReceived(IWebSocket client, WSDataFrameEventArgs e)
        {
            if (client.Client.GetFlag()=="A")
            {
                var str = e.DataFrame.ToText();
                await Console.Out.WriteLineAsync($"{this.GetType().Name},{str}");
                return;
            }
           
            await e.InvokeNext();
        }
    }

    /// <summary>
    /// WebSocket服务B
    /// </summary>
    public class WebSocketPluginB : PluginBase, IWebSocketReceivedPlugin
    {
        private ILog m_logger;

        public WebSocketPluginB(ILog logger)
        {
            this.m_logger = logger;
        }
        public async Task OnWebSocketReceived(IWebSocket client, WSDataFrameEventArgs e)
        {
            if (client.Client.GetFlag() == "B")
            {
                var str = e.DataFrame.ToText();
                await Console.Out.WriteLineAsync($"{this.GetType().Name},{str}");
                return;
            }

            await e.InvokeNext();
        }
    }

}
