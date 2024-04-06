using TouchSocket.Core;
using TouchSocket.Sockets;

namespace KDServer.WebSocket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddHttpService(config =>
            {
                config.SetListenIPHosts(9802) // 指定监听端口
                 .ConfigureContainer(a =>
                 {
                     a.AddConsoleLogger();
                 })
          .ConfigurePlugins(a =>
          {
              a.UseWebSocket()//添加WebSocket功能
                  .SetWSUrl("/ws")//设置url直接可以连接。
                     .UseAutoPong();//当收到ping报文时自动回应pong
          });
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();
            // app.UseWebSocketDmtp();

            app.MapControllers();

            app.Run();
        }
    }
}
