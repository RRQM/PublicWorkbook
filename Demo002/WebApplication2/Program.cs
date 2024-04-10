
using TouchSocket.Sockets;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args).Inject();

            builder.Services.AddControllers().AddInject();

            var app = builder.Build();

            app.UseInject();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
