
using TouchSocket.Sockets;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args).Inject();

            builder.Services.AddControllers().AddInject();

            // Add services to the container.

            builder.Services.AddSpecificationDocuments();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseInject();

            app.UseAuthorization();

            app.UseSpecificationDocuments();

            app.MapControllers();

            app.Run();
        }
    }
}
