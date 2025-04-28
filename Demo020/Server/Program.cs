using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Sockets;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var host = Startup();
            var service = host.Services.Resolve<ITcpDmtpService>();
            service.AddListen(new TcpListenOption()
            {
                Name = $"Default_TcpDmtp",
                IpHost = "127.0.0.1:9100",
            });
            host.Run();
        }

        public static IHost Startup()
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.ConfigureContainer(a =>
                    {
                        a.AddLogger(logger =>
                        {
                            logger.AddConsoleLogger();
                            logger.AddFileLogger();
                        });
                    });

                    services.AddServiceHostedService<ITcpDmtpService, TcpDmtpService>(config =>
                    {
                        SetServerConfig(config);
                    });

                }).Build();
            return host;
        }

        public static void SetServerConfig(TouchSocketConfig config)
        {
            config
                .ConfigureContainer(a =>
                {
                    a.AddConsoleLogger();
                })
                .ConfigurePlugins(a =>
                {
                    a.UseCheckClear()
                    .SetCheckClearType(CheckClearType.All)
                    .SetTick(TimeSpan.FromSeconds(60))
                    .SetOnClose(async (c, t) =>
                    {
                        await c.ShutdownAsync(SocketShutdown.Both);
                        await c.CloseAsync("Connection closed.");
                    });
                })
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Dmtp",
                });
        }

    }
}
