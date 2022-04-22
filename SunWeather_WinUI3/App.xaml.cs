using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SunWeather_WinUI3
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private MainWindow m_window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            SetupExceptionProcess();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
            base.OnLaunched(args);
        }

        // 错误处理
        private void SetupExceptionProcess()
        {
            this.UnhandledException += UnhandledExceptionProcess;
        }

        private void UnhandledExceptionProcess(object sender, UnhandledExceptionEventArgs e)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Content = $@"程序出现错误，请将此弹窗截图发送给开发者。
错误信息：
{e.Message}",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = m_window.Content.XamlRoot
            };
            contentDialog.ShowAsync();
            e.Handled = true;
        }
    }
}
