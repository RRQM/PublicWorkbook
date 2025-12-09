using System.IO;
using System.Windows;
using Markdig.Wpf;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // 配置Markdig.Wpf
            MarkdownViewer.Pipeline = new Markdig.MarkdownPipelineBuilder()
                .UseSupportedExtensions()
                .Build();
            
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            // 读取并显示ReleaseNote.md文件
            string mdFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReleaseNote.md");
            
            if (File.Exists(mdFilePath))
            {
                string markdown = File.ReadAllText(mdFilePath);
                MarkdownViewer.Markdown = markdown;
            }
            else
            {
                MessageBox.Show($"找不到文件: {mdFilePath}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}