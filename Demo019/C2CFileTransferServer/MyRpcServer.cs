using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;

namespace C2CFileTransferServer
{
    public partial class MyRpcServer : RpcServer
    {
        [Description("登录")]//服务描述，在生成代理时，会变成注释。
        [DmtpRpc(MethodInvoke = true)]//使用函数名直接调用
        public bool Login(string account, string password)
        {
            if (account == "123" && password == "abc")
            {
                return true;
            }

            return false;
        }
    }

}
