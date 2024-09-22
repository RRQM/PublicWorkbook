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

            for (int j = 0; j < 10; j++)
            {
                Task.Run(async () =>
                {
                    for (int i = 0; i < 50; i++)
                    {
                        var cli = await CreateClientAsync("127.0.0.1", "7789");
                        Clients.Add(cli);
                    }

                    if (Clients.Count > 0)
                    {
                        foreach (var c in Clients)
                        {
                            await c.TryConnectAsync();
                        }
                    }
                });
            }
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
        private async Task<TcpClient> CreateClientAsync(string IP, string Port)
        {
            var tcpClient = new TcpClient();

            //载入配置
            await tcpClient.SetupAsync(new TouchSocketConfig()
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
