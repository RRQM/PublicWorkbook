using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace WindowsFormsApp7
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();

            var service = new HttpService();
            service.Setup(new TouchSocketConfig()
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

                    a.AddHttpPlugin(async (c, e) =>
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
                            if (response.Content!=null)
                            {

                            }
                            await response.AnswerAsync().ConfigureAwait(EasyTask.ContinueOnCapturedContext);
                        }
                    });
                }));
             service.StartAsync();

            Console.WriteLine("以下连接用于测试webApi");
            Console.WriteLine($"使用：http://127.0.0.1:7789/ApiServer/Sum?a=10&b=20");
        }
    }
}
