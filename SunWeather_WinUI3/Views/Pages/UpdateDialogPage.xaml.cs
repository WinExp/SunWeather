using EasyUpdate;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SunWeather_WinUI3.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UpdateDialogPage : Page
    {
        public UpdateDialogPage()
        {
            this.InitializeComponent();
        }

        internal async Task UpdateAppAsync(UpdateInfo updateInfo)
        {
            UpdateStatusTextBlock.Text = $"正在下载 Sun Weather {updateInfo.Version}...";
            var downloadInfo = await Updater.DownloadUpdate(updateInfo);
            while (!downloadInfo.IsCompleted)
            {
                UpdateProgressPercentProgressBar.Value = Math.Ceiling(downloadInfo.DownloadedDataPercent);
                UpdateProgressPercentTextBlock.Text = Math.Ceiling(downloadInfo.DownloadedDataPercent) + "%";
                await Task.Delay(500);
            }
            UpdateProgressPercentTextBlock.Text = "已完成";
            for (int i = 5; i >= 0; i--)
            {
                UpdateStatusTextBlock.Text = $"程序将在 {i} 秒后退出...";
                await Task.Delay(1000);
            }
            await Updater.StartUpdateAsync(updateInfo);
            Environment.Exit(0);
        }
    }
}
