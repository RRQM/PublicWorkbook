using TouchSocket.Core;
using TouchSocket.Sockets;

namespace TouchSocketTestApp
{
    public partial class Main : Form
    {
        //issue:https://gitee.com/RRQM_Home/TouchSocket/issues/IAS9NG
        public Main()
        {
            InitializeComponent();
        }

        public static ConcurrentList<TcpClient> Clients;

        private void btn_Start_Click(object sender, EventArgs e)
        {
            Clients = new ConcurrentList<TcpClient>();
            Task.Run(() =>
            {
                for (int i = 0; i < 500; i++)
                {
                    var cli = CreateClient("127.0.0.1", "7789");
                    Clients.Add(cli);
                }

                if (Clients.Count > 0)
                {
                    foreach (var c in Clients)
                    {
                        _ = c.TryConnectAsync();
                    }
                }
            });
        }

        private async void btn_End_ClickAsync(object sender, EventArgs e)
        {
            if (Clients != null)
            {
                foreach (var c in Clients)
                {
                    await c.SafeCloseAsync();
                    c.SafeDispose();
                }
                await Task.Delay(2567);
            }
        }

        /// <summary>
        /// 创建Socket客户端
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <returns></returns>
        private TcpClient CreateClient(string IP, string Port)
        {
            var tcpClient = new TcpClient();

            //载入配置
            tcpClient.Setup(new TouchSocketConfig()
                  .SetRemoteIPHost($"{IP}:{Port}")
                  .ConfigureContainer(a =>
                  {
                      a.AddConsoleLogger();
                  })
                  .ConfigurePlugins(a =>
                  {
                      a.UseTcpReconnection();
                  })
                  );


            tcpClient.Logger.Info("客户端成功连接");
            return tcpClient;
        }
    }
}
