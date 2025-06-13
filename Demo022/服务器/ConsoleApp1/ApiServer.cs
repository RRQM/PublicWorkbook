using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TouchSocket.Core;
using TouchSocket.Rpc;
using TouchSocket.WebApi;

namespace WindowsFormsApp7
{
    public partial class ApiServer : SingletonRpcServer
    {
        private readonly ILog m_logger;

        public ApiServer(ILog logger)
        {
            this.m_logger = logger;
        }

        [WebApi(Method = HttpMethodType.Get)]
        public int Sum(int a, int b)
        {
            return a + b;
        }
    }
}
