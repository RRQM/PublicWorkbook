
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Sockets;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddTcpDmtpService(config =>
            {
                config.SetListenIPHosts(7789)
                .SetDmtpOption(new DmtpOption()
                {
                    VerifyToken = "Dmtp"//设定连接口令，作用类似账号密码
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            _=Task.Run(async () => 
            {
                await Task.Delay(2000);
                var client = new TcpDmtpClient();
                client.Setup(new TouchSocketConfig()
                    .SetRemoteIPHost("127.0.0.1:7789")
                    .SetDmtpOption(new DmtpOption()
                    {
                        VerifyToken = "Dmtp"
                    }));
                client.Connect();
                Console.WriteLine("s");
            });
            app.Run();
        }
    }
}
