using System;
using System.Text;
using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Sockets;

namespace ConsoleApp1
{
    internal class Program
    {

        //https://gitee.com/RRQM_Home/TouchSocket/issues/I9PXWT
        static async Task Main(string[] args)
        {
            var server = GetService();


            using (var client = new System.Net.Http.HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10);
                using (var content = new MultipartFormDataContent())
                {
                    var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("Hello"));
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");
                    content.Add(fileContent, "\"file\"", "\"中文.txt\"");

                    var response =await client.PostAsync("http://127.0.0.1:7789/file", content);
                    response.EnsureSuccessStatusCode();

                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                }
            }
        }

        static HttpService GetService()
        {
            var service = new HttpService();
            service.Setup(new TouchSocketConfig()//加载配置
                .SetListenIPHosts(7789)
                .ConfigureContainer(a =>
                {
                    a.AddConsoleLogger();
                })
                .ConfigurePlugins(a =>
                {
                    //以下即是插件
                    a.Add<MyHttpPlug1>();
                    a.UseDefaultHttpServicePlugin();
                }));

            service.Start();

            return service;
        }
    }

    class MyHttpPlug1 : PluginBase, IHttpPlugin
    {
        public async Task OnHttpRequest(IHttpSocketClient client, HttpContextEventArgs e)
        {
            var request = e.Context.Request;

            if (request.UrlEquals("/file"))
            {
                try
                {
                    string body = request.GetBody();

                    foreach (var item in request.GetMultifileCollection())
                    {
                        Console.WriteLine(item.Name);
                    }

                    await e.Context.Response
               .SetStatus()
               .AnswerAsync();
                }
                catch (Exception ex)
                {


                }

            }


            await e.InvokeNext();
        }
    }
}
