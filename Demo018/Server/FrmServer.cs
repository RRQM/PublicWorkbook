using System.Windows.Forms;
using System;
using TouchSocket.Dmtp;
using TouchSocket.Core;
using TouchSocket.Sockets;
using TouchSocket.Rpc;
using TouchSocket.Dmtp.Rpc;

namespace Server
{

    public partial class FrmServer : Form
    {
        TcpDmtpService rpcService;
        public FrmServer()
        {
            InitializeComponent();
        }

        private void FrmServer_Load(object sender, EventArgs e)
        {
            InitServer();
        }

        private void InitServer()
        {
            rpcService = new TcpDmtpService();
            var config = new TouchSocketConfig()//配置
            .SetListenIPHosts(new IPHost[] { new IPHost(7789) })
            .ConfigureContainer(a =>
            {
                a.AddRpcStore(store =>
                {
                    store.RegisterServer<MyRpcServer>();
                });
                a.AddEasyLogger(ShowMsg);
            })
            .ConfigurePlugins(a =>
            {
                a.UseDmtpRpc();
                a.UseCheckClear().SetTick(timeSpan: TimeSpan.FromSeconds(120)).SetCheckClearType(CheckClearType.OnlySend);
                a.Add(typeof(ITcpConnectedPlugin), async (ITcpSession client, ConnectedEventArgs e) =>
                {
                    //指定参数类型，此时client和e为所需类型
                    //同时如果不主动调用InvokeNext，插件将中断传递
                    rpcService.Logger.Info($"{((TcpDmtpSessionClient)client).Id} : {client.IP} 登录");
                    await e.InvokeNext();
                });
                a.Add(typeof(ITcpClosedPlugin), async (ITcpSession client, ClosedEventArgs e) =>
                {
                    rpcService.Logger.Info($"{((TcpDmtpSessionClient)client).Id} : {client.IP} 退出");
                    await e.InvokeNext();
                });

            })
            .SetDmtpOption(new DmtpOption()
            {
                 VerifyToken = "TouchRpc" 
            });

            rpcService.Setup(config);
            rpcService.Start();
            rpcService.Logger.Info($"{rpcService.GetType().Name}已启动");
        }

        private void ShowMsg(string msg)
        {
            txtLog.Invoke(new Action(() => txtLog.AppendText(msg)));
        }
    }
}
