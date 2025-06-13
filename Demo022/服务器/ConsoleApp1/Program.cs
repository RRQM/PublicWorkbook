using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Rpc;
using TouchSocket.Sockets;
using WindowsFormsApp7;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var service = new HttpService();
            await service.SetupAsync(new TouchSocketConfig()
                 .SetListenIPHosts(7789)
                 .ConfigureContainer(a =>
                 {
                     a.AddConsoleLogger();
                     a.AddRpcStore(store =>
                     {
                         store.RegisterServer<ApiServer>();//注册服务
                     });
                     //添加跨域服务
                     a.AddCors(corsOption =>
                     {
                         //添加跨域策略，后续使用policyName即可应用跨域策略。
                         corsOption.Add("cors", corsBuilder =>
                         {
                             corsBuilder.AllowAnyMethod()
                                 .AllowAnyOrigin();
                         });
                     });
                 })
                 .ConfigurePlugins(a =>
                 {
                     //a.UseCheckClear();
                     a.UseCors("cors");
                     a.UseWebApi();

                     //此插件是http的兜底插件，应该最后添加。作用是当所有路由不匹配时返回404.且内部也会处理Option请求。可以更好的处理来自浏览器的跨域探测。
                     // a.UseDefaultHttpServicePlugin();

                     a.AddHttpPlugin(async(c,e) => 
                     {
                         var response = e.Context.Response;
                         if (response.Responsed)
                         {
                             return;
                         }

                         if (e.Context.Request.IsMethod("OPTIONS"))
                         {
                             response.SetStatus(204, "No Content");
                             response.Headers.TryAdd("Access-Control-Allow-Origin", "*");
                             response.Headers.TryAdd("Access-Control-Allow-Headers", "*");
                             response.Headers.TryAdd("Allow", "OPTIONS, GET, POST");
                             response.Headers.TryAdd("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
                             //if (response.Content!=null)
                             //{
                             //    response.Content = default;
                             //}
                             await response.AnswerAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                         }
                         else
                         {
                             await response.UrlNotFind().AnswerAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                         }
                     });
                 }));
            await service.StartAsync();

            Console.WriteLine("以下连接用于测试webApi");
            Console.WriteLine($"使用：http://127.0.0.1:7789/ApiServer/Sum?a=10&b=20");
            while (true)
            {
                Console.ReadKey();
            }

        }
    }
}
