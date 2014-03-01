using System;
using System.Text;
using System.Windows;

namespace Xblero.Apps
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public App()
        {
            InitializeExceptions();
        }

        /// <summary>
        /// 初始化 <seealso cref="Application"/> 中未处理的异常。
        /// </summary>
        public void InitializeExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => DisplayException(e.ExceptionObject as Exception);
            Current.DispatcherUnhandledException += (sender, e) =>
            {
                DisplayException(e.Exception);
                e.Handled = true;
                Shutdown(-1);
            };
        }

        /// <summary>
        /// 显示应用程序无法处理的异常并退出程序。
        /// </summary>
        public static void DisplayException(Exception ex, int code = 0)
        {
#if DEBUG
            var builder = new StringBuilder();
            builder.AppendLine("如果可能，请将以下信息记录下来，方便我们改进程序。");
            builder.AppendLine();
            var e = ex;
            while (e != null)
            {
                builder.AppendLine(e.Message);
                builder.AppendLine(e.StackTrace);
                builder.AppendLine();
                e = e.InnerException;
            }
            var content = builder.ToString();
            builder.Clear();
            builder.Append("计时器遇到问题，需要关闭");
            if (code != 0)
            {
                builder.Append("。错误代码：");
                builder.Append(code);
            }
            var title = builder.ToString();
#else
            const string content = "您可以重新启动应用程序，然后继续。";
            const string title = "杯具！计时器崩溃了……";
#endif
            MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
