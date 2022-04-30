using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SunWeather_WinUI3.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpPage : Page
    {
        private Dictionary<string, string> projectsDictionary = new Dictionary<string, string>();

        public HelpPage()
        {
            this.InitializeComponent();
            projectsDictionary.Add("Windows UI Library", "https://github.com/microsoft/microsoft-ui-xaml");
            projectsDictionary.Add("Windows App SDK", "https://github.com/microsoft/WindowsAppSDK");
            projectsDictionary.Add("QWeather", "https://www.qweather.com/");
            projectsDictionary.Add("QWeatherAPI", "https://github.com/WinExp/QWeatherAPI");
        }

        private void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            OpenWebsitesWithDefaultBrowser("https://github.com/WinExp/SunWeather/issues");
        }

        private void OpenWebsitesWithDefaultBrowser(string url)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice");
            string progId = key.GetValue("ProgId").ToString();

            key = Registry.ClassesRoot.OpenSubKey($@"{progId}\shell\open\command");
            string browserPath = key.GetValue("").ToString();
            browserPath = browserPath.Substring(1, browserPath.IndexOf(".exe") + 4);

            Process.Start(browserPath, url);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            OpenWebsitesWithDefaultBrowser(projectsDictionary[e.ClickedItem.ToString()]);
        }
    }
}
