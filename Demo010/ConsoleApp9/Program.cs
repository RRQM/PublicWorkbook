using System.Text;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace ConsoleApp9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //issue https://gitee.com/RRQM_Home/TouchSocket/issues/I9GCGT
            var service = new TcpService();
            service.Received = (client, e) =>
            {
                _ = Task.Run(async () =>
                 {
                     //调用CreateWaitingClient获取到IWaitingClient的对象。
                     var waitClient = client.CreateWaitingClient(new WaitingOptions());

                     //然后使用SendThenReturn。
                     byte[] returnData = await waitClient.SendThenReturnAsync(Encoding.UTF8.GetBytes("RRQM"));
                     client.Logger.Info($"收到回应消息：{Encoding.UTF8.GetString(returnData)}");
                 });

                return EasyTask.CompletedTask;
            };

            service.Setup(new TouchSocketConfig()//载入配置     
                .SetListenIPHosts(new IPHost[] { new IPHost("tcp://127.0.0.1:17803"), new IPHost(7790) })//同时监听两个地址
                .ConfigureContainer(a =>//容器的配置顺序应该在最前面
                {
                    a.AddConsoleLogger();//添加一个控制台日志注入（注意：在maui中控制台日志不可用）
                })
                .ConfigurePlugins(a =>
                {
                    //a.Add();//此处可以添加插件
                }));

            service.Start();//启动

            Console.ReadLine();

        }


    }
}
