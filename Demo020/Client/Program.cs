using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Sockets;

namespace Client
{
    internal class Program
    {
        private static ManualResetEvent _manualReset = new ManualResetEvent(false);
        static async Task Main(string[] args)
        {
            TcpDmtpClient client = new TcpDmtpClient();
            client.Setup(GetTouchSocketConfig("127.0.0.1:9100"));
            var result = await client.TryConnectAsync();
            _manualReset.WaitOne();
        }

        public static TouchSocketConfig GetTouchSocketConfig(string host)
        {
            var config = new TouchSocketConfig()
                .SetRemoteIPHost(host)
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Dmtp"
                })
                .ConfigurePlugins(a =>
                {
                    a.UseDmtpHeartbeat()
                    .SetTick(TimeSpan.FromSeconds(5))
                    .SetMaxFailCount(3);
                    a.UseDmtpReconnection<IDmtpClient>()
                    .UsePolling(TimeSpan.FromSeconds(5))
                    .SetActionForCheck(async (c, i) =>
                    {
                        if (await c.PingAsync())
                        {
                            return true;
                        }
                        else if (DateTime.Now - c.GetLastActiveTime() < TimeSpan.FromSeconds(5))
                        {
                            return null;
                        }
                        else
                        {
                            return false;
                        }
                    });
                });
            return config;
        }
    }

}
