using Fleck;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Windows;
using System.Windows.Threading;

namespace WebSocketTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        WebSocketServer server = new WebSocketServer("ws://127.0.0.1:8888");

        public MainWindow()
        {
            InitializeComponent();

            

        
            FleckLog.Level = LogLevel.Debug;
            var allSockets = new List<IWebSocketConnection>();
          
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Open!");
                    allSockets.Add(socket);
                };
                socket.OnClose = () =>
                {
                    list.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        list.Items.Add("Close");
                    }));
                    socket.Send("客户端关闭连接"); 
                    allSockets.Remove(socket);
                };
                socket.OnMessage = message =>
                {
                   socket.Send("服务端发送: " + message); 
                //Console.WriteLine(message);
                list.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        list.Items.Add(message);
                    }));
                //        allSockets.ToList().ForEach(s => s.Send("Echo: " + message));
            };
            });

        }

      

        private void Window_Closed(object sender, EventArgs e)
        {
            
            server.Dispose();
            

        }

      
    }

}
