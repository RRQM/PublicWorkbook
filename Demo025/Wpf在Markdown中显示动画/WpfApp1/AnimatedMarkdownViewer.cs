using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using Markdig.Wpf;
using XamlAnimatedGif;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// 支持GIF动画的MarkdownViewer
    /// </summary>
    public class AnimatedMarkdownViewer : MarkdownViewer
    {
        public AnimatedMarkdownViewer()
        {
            // 监听Markdown属性变化
            var descriptor = DependencyPropertyDescriptor.FromProperty(
                MarkdownProperty,
                typeof(MarkdownViewer));

            descriptor?.AddValueChanged(this, (s, e) =>
            {
                // Markdown内容改变后，延迟处理GIF
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    System.Diagnostics.Debug.WriteLine("=== Markdown changed, processing images ===");
                    ProcessImages(this);
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            });

            // 也监听控件加载完成事件
            this.Loaded += (s, e) =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    System.Diagnostics.Debug.WriteLine("=== Control loaded, processing images ===");
                    ProcessImages(this);
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            };

            // 监听布局更新
            this.LayoutUpdated += (s, e) =>
            {
                // 只在有新内容时处理
                if (this.Document != null)
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ProcessImages(this);
                    }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                }
            };
        }

        private void ProcessImages(DependencyObject element)
        {
            if (element == null) return;

            // 如果是Image控件，检查是否是GIF
            if (element is Image image)
            {
                System.Diagnostics.Debug.WriteLine($"Found Image control, Source type: {image.Source?.GetType().Name}");
                
                if (image.Source is BitmapImage bitmapImage)
                {
                    var uri = bitmapImage.UriSource;
                    System.Diagnostics.Debug.WriteLine($"BitmapImage URI: {uri}");
                    
                    if (uri != null && uri.ToString().EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            // 转换为绝对URI
                            Uri absoluteUri = uri;
                            if (!uri.IsAbsoluteUri)
                            {
                                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                                var relativePath = uri.ToString().TrimStart('.', '/', '\\');
                                absoluteUri = new Uri(System.IO.Path.Combine(baseDir, relativePath));
                                System.Diagnostics.Debug.WriteLine($"Converted to absolute URI: {absoluteUri}");
                            }

                            // 清除原有的Source，使用AnimationBehavior接管
                            image.Source = null;
                            AnimationBehavior.SetSourceUri(image, absoluteUri);
                            
                            // 设置其他可能需要的属性
                            AnimationBehavior.SetAutoStart(image, true);
                            AnimationBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"✗ Error applying GIF animation: {ex.Message}");
                        }
                    }
                }
            }

            // 递归处理子元素
            var childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                ProcessImages(child);
            }
        }
    }
}



