using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace ClientDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient? TcpClient { get; set; }

        private Task? RefreshTask { get; set; }

        private CancellationTokenSource? CancellationTokenSource { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CancellationTokenSource = new CancellationTokenSource();

            TcpClient = new TcpClient();
            TcpClient.Setup(new TouchSocketConfig()
                .SetRemoteIPHost("127.0.0.1:12345")
                .SetTcpDataHandlingAdapter(() => new FixedHeaderPackageAdapter())
                .ConfigureContainer(a =>
                {
                    a.AddFileLogger();
                }).ConfigurePlugins(a =>
                {
                    a.UseReconnection(5, true, 1000, client =>
                    {
                        CancellationTokenSource.Cancel();
                        try
                        {
                            RefreshTask?.Wait();
                            CancellationTokenSource.Dispose();
                        }
                        catch (AggregateException)
                        {
                            // ignored
                        }
                        catch (TaskCanceledException)
                        {
                            // ignored
                        }
                        CancellationTokenSource = new CancellationTokenSource();
                        RefreshTask = RefreshNodeServer(client, CancellationTokenSource.Token);
                    });
                }));
            TcpClient.Connect();
            RefreshTask = RefreshNodeServer(TcpClient, CancellationTokenSource.Token);
        }

        Task RefreshNodeServer(ITcpClient tcpClient, CancellationToken cts)
        {
            return Task.Run(async () =>
            {
                var text = "test";
                while (true)
                {
                    if (cts.IsCancellationRequested) return;
                    await Task.Delay(1000, cts);
                    try
                    {
                        await tcpClient.SendAsync(text);
                    }
                    catch (Exception e)
                    {
                        await Console.Out.WriteLineAsync(e.Message);
                    }
                }
            }, cts);
        }
    }
}