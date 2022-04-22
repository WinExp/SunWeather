using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using QWeatherAPI.Result.GeoAPI.CityLookup;
using QWeatherAPI.Result.RealTimeWeather;
using SunWeather_WinUI3.Class;
using SunWeather_WinUI3.Class.Tools.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SunWeather_WinUI3.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        private readonly Dictionary<string, string> qweatherToSeniverseIcon = new Dictionary<string, string>();

        public HomePage()
        {
            this.InitializeComponent();
            MainWindow.homePage = this;
            int hour = DateTime.Now.Hour;
            string title;
            string tip;
            if (hour >= 0 && hour < 5)
            {
                title = "凌晨";
                tip = "别熬夜了，去睡觉吧💤";
            }
            else if (hour >= 5 && hour < 8)
            {
                title = "早上";
                tip = "一日之际在于晨☀";
            }
            else if (hour >= 8 && hour < 12)
            {
                title = "上午";
                tip = "去干点什么事情吧，也没准你还有什么事情没干呢qwq🦥";
            }
            else if (hour >= 12 && hour < 13)
            {
                title = "中午";
                tip = "你确定不去睡个午觉？💤";
            }
            else if (hour >= 13 && hour < 18)
            {
                title = "下午";
                tip = "来个惬意的下午茶吧🍵";
            }
            else
            {
                title = "晚上";
                tip = "忙碌的一天结束了，来休息下吧🌃";
            }
            this.Loaded += delegate
            {
                string tips = $"{title}好, {Environment.UserName}。{tip}";
                ShowTipsAsync(tips);
            };

            // 加载 QWeather 天气图标 ID 至 Seniverse 天气图标 ID 的转换字典
            string[] qweatherIcon = { "100", "101", "102",
            "103", "104", "150",
            "151", "152", "153",
            "154", "300", "301",
            "302", "303", "304",
            "305", "306", "307",
            "308", "309", "310",
            "311", "312", "313",
            "314", "315", "316",
            "317", "318", "350",
            "351", "399", "400",
            "401", "402", "403",
            "404", "405", "406",
            "407", "408", "409",
            "410", "456", "457",
            "499", "500", "501",
            "502", "503", "504",
            "507", "508", "509",
            "510", "511", "512",
            "513", "514", "515",
            "900", "901", "999"};
            string[] seniverseIcon = { "0", "4", "4",
            "5", "9", "1",
            "4", "4", "6",
            "9", "10", "10",
            "11", "11", "12",
            "13", "14", "15",
            "18", "13", "16",
            "17", "18", "19",
            "14", "15", "16",
            "17", "18", "10",
            "10", "10", "22",
            "23", "24", "25",
            "20", "20", "20",
            "21", "23", "24",
            "25", "20", "21",
            "21", "30", "30",
            "31", "27", "26",
            "28", "29", "30",
            "30", "31", "31",
            "31", "30", "30",
            "38", "37", "99" };
            for (int i = 0; i < qweatherIcon.Length; i++)
            {
                qweatherToSeniverseIcon.Add(qweatherIcon[i], seniverseIcon[i]);
            }
        }

        private async Task ShowTipsAsync(string tips)
        {
            await Task.Delay(500);
            this.TipTextBlock.Text = tips;
            this.TipTextBlock.Opacity = 1;
        }

        internal async Task QueryWeatherAsync(Location location)
        {
            this.SearchLocationTipTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            this.WeatherInfoFrame.Opacity = 0;
            this.WeatherInfoFrame.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            this.WeatherProgressRing.IsActive = true;
            WeatherResult weather = await QWeatherAPI.RealTimeWeatherAPI.GetRealTimeWeatherAsync(location.Lon, location.Lat, Configs.ApiKey, Configs.Unit);
            string adm = location.Adm2.Substring(0, location.Name.Length >= location.Adm2.Length ?
                location.Adm2.Length : location.Name.Length) == location.Name ? location.Adm1 : location.Adm2;
            string locationName = $"{adm}, {location.Name}";
            this.WeatherTitleTextBlock.Text = $"{locationName} 的天气";
            this.WeatherImage.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Seniverse/Icon/{qweatherToSeniverseIcon[weather.Now.Icon]}@2x.png"));
            string tempUnit;
            switch (Configs.Unit)
            {
                case QWeatherAPI.Tools.Units.Metric:
                    tempUnit = "°C";
                    break;
                case QWeatherAPI.Tools.Units.Inch:
                    tempUnit = "°F";
                    break;
                default:
                    tempUnit = "°C";
                    break;
            }
            this.TempTextBlock.Text = $"{weather.Now.Temp}{tempUnit}";
            this.FeelsLikeTempTextBlock.Text = $"感觉像 {weather.Now.FeelsLike}{tempUnit}";
            this.WeatherDescriptionTextBlock.Text = weather.Now.Text;
            this.WeatherProgressRing.IsActive = false;
            this.WeatherInfoFrame.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            this.WeatherInfoFrame.Opacity = 1;
        }
    }
}