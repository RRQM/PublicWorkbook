using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;

namespace C2CFileTransferClient
{
    internal partial class MyClientRpcServer : RpcServer
    {
        private readonly ILog m_logger;

        public MyClientRpcServer(ILog logger)
        {
            this.m_logger = logger;
        }

        [DmtpRpc(MethodInvoke = true)]//使用函数名直接调用
        public bool Notice(string msg)
        {
            this.m_logger.Info(msg);
            return true;
        }
    }
}
