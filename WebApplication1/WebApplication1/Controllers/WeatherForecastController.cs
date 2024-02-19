using Microsoft.AspNetCore.Mvc;
using TouchSocket.Dmtp;
using TouchSocket.Sockets;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITcpDmtpService m_tcpDmtpService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,ITcpDmtpService tcpDmtpService)
        {
            _logger = logger;
            this.m_tcpDmtpService = tcpDmtpService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<string> Get()
        {
           return m_tcpDmtpService.GetIds();
        }
    }
}
