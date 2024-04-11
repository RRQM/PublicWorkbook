using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Core;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;

namespace ServerForm
{
    class MyRpcServer : TransientRpcServer
    {
        private readonly ILog m_logger;
        public MyRpcServer(ILog logger)
        {
            this.m_logger = logger;
        }

        [Description("发送客户端数据并检查是否重复登录")]//服务描述，在生成代理时，会变成注释。
        [DmtpRpc("FormClientInfoToServerListAsync")]//服务注册的函数键，此处为显式指定。默认不传参的时候，为该函数类全名+方法名的全小写。

        public async Task<bool> FormClientInfoToServerListAsync(string clientId,  string macAddress, string computerName)
        {

            var result = await AppMessenger.Default.SendAsync<bool>("ClientInfoToServerListAsync", clientId, macAddress, computerName);
            m_logger.Info($"{clientId} : 网卡:  {macAddress} 主机名 {computerName} ");
            return result;
        }
    }
}
