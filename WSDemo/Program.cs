using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Rpc;
using TouchSocket.Sockets;
using TouchSocket.WebApi.Swagger;

namespace WSDemo
{
    internal class Program
    {
        // issue:https://gitee.com/RRQM_Home/TouchSocket/issues/I9BUKF
        private static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            //builder.Services.AddHostedService<Worker>();
            //统一配置容器
            builder.Services.ConfigureContainer(a =>
            {
                a.AddConsoleLogger();
            });

            //HTTP服务器
            builder.Services.AddServiceHostedService<IHttpService, HttpService>(config =>
            {
                config.SetListenIPHosts(8084)
                .ConfigurePlugins(a =>
                {
                    //WebSocket服务器A
                    a.UseWebSocket()            //添加WebSocket功能
                    .SetVerifyConnection(VerifyConnection)
                    .UseAutoPong();             //当收到ping报文时自动回应pong

                    a.Add<WebSocketPluginA>();
                    a.Add<WebSocketPluginB>();
                });
            });

            var host = builder.Build();
            host.Run();

        }

        private static bool VerifyConnection(IHttpSocketClient client, HttpContext context)
        {
            if (!context.Request.IsUpgrade())//如果不包含升级协议的header，就直接返回false。
            {
                return false;
            }
            if (context.Request.UrlEquals("/wsa"))//以A连接
            {
                client.SetFlag("A");
                return true;
            }
            else if (context.Request.UrlEquals("/wsb"))//以B连接
            {
                client.SetFlag("B");
                return true;
            }
            return false;

        }
    }

}