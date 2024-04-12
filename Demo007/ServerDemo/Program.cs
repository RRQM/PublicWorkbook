// See https://aka.ms/new-console-template for more information
using System.Text;
using TouchSocket.Core;
using TouchSocket.Sockets;

var service = new TcpService();

service.Setup(new TouchSocketConfig()
    .SetListenIPHosts(12345)
    .SetTcpDataHandlingAdapter(() => new FixedHeaderPackageAdapter())
    .SetThreadCount(200)
    .ConfigureContainer(a =>
    {
        a.AddLogger(new LoggerGroup(ConsoleLogger.Default, new FileLogger()));
    })
    .ConfigurePlugins(a =>
    {
        a.UseCheckClear()
            .SetCheckClearType(CheckClearType.All)
            .SetTick(TimeSpan.FromSeconds(120))
            .SetOnClose((c, t) =>
            {
                c.TryShutdown();
                c.SafeClose("超时无数据");
            });
    }));

service.Connected = async (client, _) =>
{
    using (var receiver = client.CreateReceiver())
    {
        while (true)
        {
            try
            {
                using (var receiverResult = await receiver.ReadAsync(CancellationToken.None))
                {
                    try
                    {
                        if (receiverResult.IsClosed)
                        {
                            return;
                        }
                        receiverResult.ByteBlock.Read(out var data, receiverResult.ByteBlock.Len);//先正常接收几次消息，然后断点打这，10秒钟左右再按F5继续，就会触发所说的BUG
                        var mes = Encoding.UTF8.GetString(data);
                        client.Logger.Info(mes);
                    }
                    catch (Exception exx)
                    {

                        
                    }
                    
                }
            }
            catch (Exception ex)
            {

               
            }
            
        }
    }
};

service.Disconnected = (client, _) =>
{
    client.Logger.Info($"{client.Id}已断线");
    return EasyTask.CompletedTask;
};

service.Start();

Console.ReadLine();