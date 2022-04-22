using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QWeatherAPI.Tools;
using SunWeather_WinUI3.Class;
using SunWeather_WinUI3.Class.Tools.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SunWeather_WinUI3.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingPage : Page
    {

        public SettingPage()
        {
            InitializeComponent();

            // 按照配置文件更改控件状态
            this.Loaded += delegate
            {
                ApiKeyPasswordBox.Password = Configs.ApiKey == Configs.DefaultApiKey ? "" : Configs.ApiKey;

                switch (Configs.Unit)
                {
                    case Units.Metric:
                        MetricRadioButton.IsChecked = true;
                        break;
                    case Units.Inch:
                        InchRadioButton.IsChecked = true;
                        break;
                }

                PreLoadAPIToggleSwitch.IsOn = Configs.PreLoadAPI;
            };
        }

        private void ChangeSaveConfigButtonStatus()
        {
            if (!this.IsLoaded)
            {
                return;
            }
            Units unit;
            if ((bool)MetricRadioButton.IsChecked)
            {
                unit = Units.Metric;
            }
            else
            {
                unit = Units.Inch;
            }

            string key = ApiKeyPasswordBox.Password == "" ? Configs.DefaultApiKey : ApiKeyPasswordBox.Password;
            if (key != Configs.ApiKey
                || unit != Configs.Unit
                || PreLoadAPIToggleSwitch.IsOn != Configs.PreLoadAPI)
            {
                SaveConfigButton.Opacity = 1;
                return;
            }
            SaveConfigButton.Opacity = 0;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ChangeSaveConfigButtonStatus();
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ChangeSaveConfigButtonStatus();
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if ((bool)checkBox.IsChecked)
            {
                ApiKeyPasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                ApiKeyPasswordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }

        private void ApiKeyPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ChangeSaveConfigButtonStatus();
        }

        private void SaveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if ((button.Content as ProgressRing) != null)
            {
                return;
            }

            using (ConfigWriter configWriter = new ConfigWriter(Configs.ConfigFilePath))
            {
                configWriter.SetConfigValue("ApiKey", ApiKeyPasswordBox.Password == "" ? Configs.DefaultApiKey : ApiKeyPasswordBox.Password);

                RadioButton radioButton;
                if ((bool)MetricRadioButton.IsChecked)
                {
                    radioButton = MetricRadioButton;
                }
                else
                {
                    radioButton = InchRadioButton;
                }
                configWriter.SetConfigValue("Unit", radioButton.Tag.ToString());

                if (PreLoadAPIToggleSwitch.IsOn)
                {
                    configWriter.SetConfigValue("PreLoadAPI", "True");
                }
                else
                {
                    configWriter.SetConfigValue("PreLoadAPI", "False");
                }
                configWriter.WriteInFile();
            }
            Configs.LoadConfig();
            ChangeSaveConfigButtonStatus();
        }
    }
}
