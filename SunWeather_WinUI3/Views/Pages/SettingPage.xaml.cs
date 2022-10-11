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
        internal static MainWindow mainWindow;

        public SettingPage()
        {
            InitializeComponent();

            // 按照配置文件更改控件状态
            this.Loaded += delegate
            {
                switch (Configs.Unit)
                {
                    case Units.Metric:
                        MetricRadioButton.IsChecked = true;
                        break;
                    case Units.Inch:
                        InchRadioButton.IsChecked = true;
                        break;
                }

                ApiKeyPasswordBox.Password = Configs.ApiKey == Configs.DefaultApiKey ? "" : Configs.ApiKey;

                int refreshDelay = Configs.AutoRefreshDelay;
                if (refreshDelay == -1)
                {
                    OffAutoRefreshRadioButton.IsChecked = true;
                }
                else if (refreshDelay == 5)
                {
                    FiveMinuteAutoRefreshRadioButton.IsChecked = true;
                }
                else if (refreshDelay == 10)
                {
                    TenMinuteAutoRefreshRadioButton.IsChecked = true;
                }
                else
                {
                    CustomMinuteAutoRefreshRadioButton.IsChecked = true;
                    CustomMinuteAutoRefreshNumberBox.Text = refreshDelay.ToString();
                }

                AutoUpdateToggleSwitch.IsOn = Configs.IsAutoUpdate;
                TrayToggleSwitch.IsOn = Configs.IsTray;
                ToastRainCheckBox.IsChecked = Configs.IsToastRain;
                ToastWarningCheckBox.IsChecked = Configs.IsToastWarning;
            };
        }

        private void UpdateSaveButtonStatus(double delay = 0)
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

            if ((bool)OffAutoRefreshRadioButton.IsChecked)
            {
                delay = -1;
            }
            else if ((bool)FiveMinuteAutoRefreshRadioButton.IsChecked)
            {
                delay = 5;
            }
            else if ((bool)TenMinuteAutoRefreshRadioButton.IsChecked)
            {
                delay = 10;
            }
            else
            {
                delay = delay == 0 ? int.Parse(CustomMinuteAutoRefreshNumberBox.Text) : delay;
            }

            string key = ApiKeyPasswordBox.Password == "" ? Configs.DefaultApiKey : ApiKeyPasswordBox.Password;
            if (key != Configs.ApiKey
                || unit != Configs.Unit
                || delay != Configs.AutoRefreshDelay
                || AutoUpdateToggleSwitch.IsOn != Configs.IsAutoUpdate
                || TrayToggleSwitch.IsOn != Configs.IsTray
                || ToastRainCheckBox.IsChecked != Configs.IsToastRain
                || ToastWarningCheckBox.IsChecked != Configs.IsToastWarning)
            {
                SaveConfigButton.Opacity = 1;
                return;
            }
            SaveConfigButton.Opacity = 0;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)CustomMinuteAutoRefreshRadioButton.IsChecked)
            {
                CustomMinuteAutoRefreshNumberBox.IsEnabled = true;
            }
            else
            {
                CustomMinuteAutoRefreshNumberBox.IsEnabled = false;
            }

            UpdateSaveButtonStatus();
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
            UpdateSaveButtonStatus();
        }

        private void CustomMinuteAutoRefreshNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            UpdateSaveButtonStatus(args.NewValue);
        }

        private void SaveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if ((button.Content as ProgressRing) != null)
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

            int delay;
            if ((bool)OffAutoRefreshRadioButton.IsChecked)
            {
                delay = -1;
            }
            else if ((bool)FiveMinuteAutoRefreshRadioButton.IsChecked)
            {
                delay = 5;
            }
            else if ((bool)TenMinuteAutoRefreshRadioButton.IsChecked)
            {
                delay = 10;
            }
            else
            {
                delay = int.Parse(CustomMinuteAutoRefreshNumberBox.Text);
            }

            Configs.Unit = unit;
            Configs.ApiKey = ApiKeyPasswordBox.Password == "" ? Configs.DefaultApiKey : ApiKeyPasswordBox.Password;
            Configs.AutoRefreshDelay = delay;
            Configs.IsAutoUpdate = AutoUpdateToggleSwitch.IsOn;
            Configs.IsTray = TrayToggleSwitch.IsOn;
            Configs.IsToastRain = (bool)ToastRainCheckBox.IsChecked;
            Configs.IsToastWarning = (bool)ToastWarningCheckBox.IsChecked;
            Configs.WriteConfig();

            UpdateSaveButtonStatus();
        }

        private void AutoUpdateToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            UpdateSaveButtonStatus();
        }

        private void TraySwitch_Toggled(object sender, RoutedEventArgs e)
        {
            UpdateSaveButtonStatus();
        }

        private async void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await mainWindow.CheckUpdateAsync();
        }

        private void UpdateToastCheckBoxesStatus()
        {
            if ((bool)ToastRainCheckBox.IsChecked
                && (bool)ToastWarningCheckBox.IsChecked)
            {
                ToastCheckBox.IsChecked = true;
            }
            else if (!(bool)ToastRainCheckBox.IsChecked
                && !(bool)ToastWarningCheckBox.IsChecked)
            {
                ToastCheckBox.IsChecked = false;
            }
            else
            {
                ToastCheckBox.IsChecked = null;
            }
        }

        private void ToastCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ToastRainCheckBox.IsChecked = true;
            ToastWarningCheckBox.IsChecked = true;
            UpdateSaveButtonStatus();
        }

        private void ToastCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ToastRainCheckBox.IsChecked = false;
            ToastWarningCheckBox.IsChecked = false;
            UpdateSaveButtonStatus();
        }

        private void ToastCheckBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            if ((bool)ToastRainCheckBox.IsChecked
                && (bool)ToastWarningCheckBox.IsChecked)
            {
                ToastCheckBox.IsChecked = false;
            }
            UpdateSaveButtonStatus();
        }

        private void ToastRainCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateToastCheckBoxesStatus();
            UpdateSaveButtonStatus();
        }

        private void ToastRainCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateToastCheckBoxesStatus();
            UpdateSaveButtonStatus();
        }

        private void ToastWarningCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateToastCheckBoxesStatus();
            UpdateSaveButtonStatus();
        }

        private void ToastWarningCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateToastCheckBoxesStatus();
            UpdateSaveButtonStatus();
        }
    }
}
