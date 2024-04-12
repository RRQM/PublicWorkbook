using Microsoft.AspNetCore.Mvc;
using TouchSocket.Sockets;

namespace Demo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TcpClientController : ControllerBase
    {
        private readonly ITcpClient m_tcpClient;

        public TcpClientController(ITcpClient tcpClient)
        {
            this.m_tcpClient = tcpClient;
        }

        [HttpGet]
        public void Send()
        {
            m_tcpClient.Connect();
            m_tcpClient.Send("Hello");
        }
    }
}
