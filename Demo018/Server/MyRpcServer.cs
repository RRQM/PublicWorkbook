
using System.ComponentModel;
using TouchSocket.Core;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;

namespace Server
{
    public partial class MyRpcServer : TransientRpcServer
    {

        private readonly ILog m_logger;
        public MyRpcServer(ILog logger)
        {
            this.m_logger = logger;
        }

        [Description("获取服务器数据")]//服务描述，在生成代理时，会变成注释。
        [DmtpRpc("GetData")]//服务注册的函数键，此处为显式指定。默认不传参的时候，为该函数类全名+方法名的全小写。

        public object GetData(List<object> data)
        {
            foreach (var item in data)
            {
                m_logger.Info(item.GetType().Name);
            }
             
            return default;
        }

    }
}
