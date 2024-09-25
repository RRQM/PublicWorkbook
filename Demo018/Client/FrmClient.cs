using Microsoft.VisualBasic.ApplicationServices;
using System.Data;
using TouchSocket.Core;
using TouchSocket.Dmtp;
using TouchSocket.Dmtp.Rpc;
using TouchSocket.Rpc;
using TouchSocket.Sockets;

namespace Client
{
    public partial class FrmClient : Form
    {
        static TcpDmtpClient client;
        public FrmClient()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            InitClient();
        }

        private void InitClient()
        {
            client = new TcpDmtpClient();
            client.Setup(new TouchSocketConfig()
                   .SetRemoteIPHost(new IPHost($"127.0.0.1:7789"))
                   .ConfigureContainer(a =>
                   {
                       //a.AddRpcStore(store =>
                       //{
                       //    store.RegisterServer<ReverseCallbackServer>();//注册服务
                       //});
                       ////  a.SetSingletonLogger(new LoggerGroup(new EasyLogger(this.ShowMsg), new FileLogger())); 
                       //a.AddLogger(new MyLogUtilLogger());//添加 MyLogUtilLogger日志
                       a.AddEasyLogger(ShowMsg);
                   })
                   .ConfigurePlugins(a =>
                   {

                       a.UseDmtpRpc()
                       //.SetSerializationSelector(new DefaultSerializationSelector()
                       //{
                       //    //仅示例，实际使用时，请赋值有效值 
                       //    JsonSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() { Formatting = Newtonsoft.Json.Formatting.None }
                       //})
                       ;
                       //a.Add<MyTouchClientRpcPlugin>();
                       a.UseDmtpHeartbeat().SetTick(timeSpan: TimeSpan.FromSeconds(60)).SetMaxFailCount(5);

                       a.UseTcpReconnection<ITcpClient>()
                       .UsePolling(TimeSpan.FromSeconds(5));//使用轮询
                       a.Add(typeof(ITcpConnectedPlugin), async (ITcpSession client, ConnectedEventArgs e) =>
                       {
                           client.Logger.Info($"连接服务器 {client.IP} 成功");
                           await e.InvokeNext();
                       });
                       a.Add(typeof(ITcpClosedPlugin), async (ITcpSession client, ClosedEventArgs e) =>
                       {
                           client.Logger.Info($"{client.IP} {e.Message}");
                           await e.InvokeNext();
                       });



                   }
                   ).SetDmtpOption(new DmtpOption()
                   {
                       VerifyToken = "TouchRpc"
                   }));
            client.Connect();

        }

        private void ShowMsg(string msg)
        {
            txtLog.Invoke(new Action(() => txtLog.AppendText(msg)));
        }
        private void btnTest_Click(object sender, EventArgs e)
        {
            var data =   new List<object> { "User", "Password", 123 };
            foreach (var item in data)
            {
                ShowMsg(item.GetType().Name+"   ");
            }

            var obj = GetData<object>(data);
        }

        public static T GetData<T>( List<object> data = null)
        {
            var dmtpInvokeOption = new DmtpInvokeOption()//调用配置
            {
                FeedbackType = FeedbackType.WaitInvoke,//调用反馈类型 
                SerializationType = SerializationType.Json,//序列化类型 
                Timeout = 3000 //调用超时设置          
            };
         
            client.GetDmtpRpcActor().InvokeT<T>("GetData", dmtpInvokeOption, data);
            return default;
        }
    }
}
