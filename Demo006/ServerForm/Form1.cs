using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TouchSocket.Core;
using TouchSocket.Rpc;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Sockets; 
using TouchSocket.Dmtp;

namespace ServerForm
{
    public partial class Form1 : Form, IMessageObject
    {
        private TcpDmtpService rpcService;

        public Form1()
        {
            InitializeComponent();
          
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitService();
        }
        private void InitService()
        {
            if (rpcService is null)
            {
                rpcService = new TcpDmtpService();
                var config = new TouchSocketConfig()//配置
                       .SetListenIPHosts(new IPHost[] { new IPHost(7789) })
                       .ConfigureContainer(a =>
                       {
                           a.AddEasyLogger(ShowMsg);
                           a.AddRpcStore(store =>
                           {
                               store.RegisterServer<MyRpcServer>();
                           });
                       })
                       .ConfigurePlugins(a =>
                       {
                           a.UseDmtpRpc();
                           a.UseCheckClear().SetTick(timeSpan: TimeSpan.FromSeconds(120)).SetCheckClearType(CheckClearType.OnlySend);

                           a.Add(nameof(IDmtpHandshakedPlugin.OnDmtpHandshaked), (ITcpDmtpSocketClient client) => 
                           {
                               client.GetDmtpRpcActor().Invoke("test", InvokeOption.WaitInvoke);
                               rpcService.Logger.Info($"{client.Id} : {client.IP} 登录");
                           });
                       })
                        .SetDmtpOption(new DmtpOption()
                        {
                            VerifyToken = "TouchRpc"
                        });

                rpcService.Setup(config);
                rpcService.Start();

                rpcService.Logger.Info($"{rpcService.GetType().Name}已启动");

                //rpcService.Connected = M_service_Connected;   
                
            }
        }
        //private Task M_service_Connected(SocketClient client, TouchSocketEventArgs e)
        //{ 
        //    //foreach (var item in rpcService.GetClients())
        //    //{
        //    //    if (item.Id == client.Id)
        //    //    {
        //    //     ((TcpDmtpSocketClient)item).GetDmtpRpcActor().Invoke("test", InvokeOption.WaitInvoke);
        //    //    }
        //    //}

        //    ((ITcpDmtpSocketClient)client).GetDmtpRpcActor().Invoke("test", InvokeOption.WaitInvoke);
        //    rpcService.Logger.Info($"{client.Id} : {client.IP} 登录");
        //    return EasyTask.CompletedTask;
        //}
        void ShowMsg(string msg)
        {
            txtLog.Invoke(new Action(() => txtLog.AppendText(msg)));
        }

        [AppMessage]
        public async Task<bool> ClientInfoToServerListAsync(string clientId, string macAddress, string computerName)
        {
            await Task.CompletedTask;
            bool result = false;
            if (macAddress=="1")
            {
                ShowMsg($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")} Id:{clientId} 获得 {macAddress} 信息成功.\r\n");
                result = true;
            }
            return result;
        }


    }
}
