using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;

namespace ClientConsole
{
    class ReverseCallbackServer : RpcServer
    {
        private readonly ILog m_logger;
        public ReverseCallbackServer(ILog logger)
        {
            this.m_logger = logger;
        }
        /// <summary>
        /// 通过RPC方式发送用户及Client信息
        /// </summary>
        [DmtpRpc(true)]//使用方法名作为调用键
        public async Task test()
        {
            bool result = false; 
            result = await Program.PutClientInfoToServerListAsync("aa", "1", "computer");
            if (!result)
                m_logger.Error("执行服务器发送用户及Client信息失败");
            return;

        }
    }
}
