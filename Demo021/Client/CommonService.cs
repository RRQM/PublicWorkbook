using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;

namespace Client
{
    internal class CommonService : SingletonRpcServer
    {
        [DmtpRpc(MethodInvoke = true)]
        public string GetInfo(byte[] data)
        {
            Thread.Sleep(1000);
            return "Info:DDD";
        }
    }
}
