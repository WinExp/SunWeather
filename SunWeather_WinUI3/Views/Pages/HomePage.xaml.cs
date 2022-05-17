using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using QWeatherAPI.Result.GeoAPI.CityLookup;
using SunWeather_WinUI3.Class;
using SunWeather_WinUI3.Class.API.AmapWebAPI.GeoIP;
using SunWeather_WinUI3.Class.API.JinrishiciAPI.Shici;
using SunWeather_WinUI3.Class.API.JinrishiciAPI.Token;
using System;
using System.Collections.Generic;
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
        private Location queryLocation;
        private Task autoRefreshTask;
        private volatile bool autoRefreshTaskCancel = true;
        internal static bool CustomLocation = false;

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
                tip = "快去干事情，懒猪qwq🦥";
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

            autoRefreshTask = new Task(async () =>
            {
                while (true)
                {
                    if (queryLocation == null || autoRefreshTaskCancel || Configs.AutoRefreshDelay == -1)
                    {
                        continue;
                    }
                    await Task.Delay(new TimeSpan(0, Configs.AutoRefreshDelay, 0));
                    if (autoRefreshTaskCancel)
                    {
                        continue;
                    }
                    this.DispatcherQueue.TryEnqueue(() =>
                    {
                        QueryWeatherAsync(queryLocation);
                    });
                }
            });
            autoRefreshTask.Start();

            GeoIPAsync();
        }

        private async Task GeoIPAsync()
        {
            StartLoading();

            try
            {
                /*string ip = await GetIP.GetIPAsync();
                var geoIpResult = await GeoIP.QueryGeoIPAsync("50lWG51WTQrGhBEaHYwnM6p2rnP5fIFn", ip);

                var geoResult = geoIpResult.Content.Address_Detail.Province == null ?
                    await QWeatherAPI.GeoAPI.GetGeoAsync(geoIpResult.Content.Address_Detail.City, Configs.ApiKey) :
                    await QWeatherAPI.GeoAPI.GetGeoAsync(geoIpResult.Content.Address_Detail.City, Configs.ApiKey, adm: geoIpResult.Content.Address_Detail.Province);*/
                if (!CustomLocation)
                {
                    var geoIpResult = await GetGeoIP.GetGeoIPAsync("4db918163480b7772263766d23b5f577");
                    var geoResult = await QWeatherAPI.GeoAPI.GetGeoAsync(geoIpResult.City, Configs.ApiKey, adm: geoIpResult.Province);
                    await QueryWeatherAsync(geoResult.Locations[0]);
                }
            }
            catch
            {
                StopLoading();

                SearchLocationTipTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                SearchLocationTipTextBlock.Opacity = 1;

                WeatherInfoFrame.Opacity = 0;
                WeatherInfoFrame.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }

        private async Task ShowTipsAsync(string tips)
        {
            await Task.Delay(500);
            this.TipTextBlock.Text = tips;
            this.TipTextBlock.Opacity = 1;
        }

        internal void StartLoading()
        {
            RefreshButton.Opacity = 0;
            RefreshButton.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            SearchLocationTipTextBlock.Opacity = 0;
            this.SearchLocationTipTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            this.WeatherInfoFrame.Opacity = 0;
            this.WeatherInfoFrame.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            autoRefreshTaskCancel = false;
            this.WeatherProgressRing.IsActive = true;
        }

        internal async void StopLoading()
        {
            this.WeatherProgressRing.IsActive = false;
            this.WeatherInfoFrame.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            this.WeatherInfoFrame.Opacity = 1;
            RefreshButton.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            RefreshButton.IsEnabled = false;
            RefreshButton.Opacity = 1;
            autoRefreshTaskCancel = false;

            await Task.Delay(3000);
            RefreshButton.IsEnabled = true;
        }

        internal async Task QueryWeatherAsync(Location location)
        {
            StartLoading();

            try
            {
                QWeatherAPI.Result.RealTimeWeather.WeatherResult weather =
                    await QWeatherAPI.RealTimeWeatherAPI.GetRealTimeWeatherAsync(location.Lon, location.Lat, Configs.ApiKey, Configs.Unit);
                string adm = location.Name;
                if (!(location.Adm2.Substring(0, location.Adm2.Length >= location.Name.Length ?
                    location.Name.Length : location.Adm2.Length) == location.Name))
                {
                    adm = location.Adm2;
                }
                else if (!(location.Adm1.Substring(0, location.Adm1.Length >= location.Name.Length ?
                    location.Name.Length : location.Adm1.Length) == location.Name))
                {
                    adm = location.Adm1;
                }
                else
                {
                    adm = location.Country;
                }
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

                ShiciTextBlock.Text = await ShiciAPI.QueryShiciAPIAsync(await TokenAPI.QueryTokenAsync());

                var dailyforecastWeatherResult =
                    await QWeatherAPI.WeatherDailyForecastAPI.GetWeatherDailyForecastAsync(location.Lon, location.Lat, Configs.ApiKey, dailyCount: QWeatherAPI.WeatherDailyForecastAPI.DailyCount._7Day);
                var daily = dailyforecastWeatherResult.Daily[0];
                SunriseTextBlock.Text = $"{daily.Sunrise} 日出";
                SunsetTextBlock.Text = $"{daily.Sunset} 日落";
                MaxMinTempTextBlock.Text = $"最高温 {daily.TempMax}{tempUnit}，最低温 {daily.TempMin}{tempUnit}";
                WindScaleTextBlock.Text = $"风速 {daily.WindScaleDay.ScaleMin}-{daily.WindScaleDay.ScaleMax} 级";
                WindDirTextBlock.Text = daily.WindDirDay;

                DailyForecastStackPanel.Children.Clear();

                for (int i = 0; i < dailyforecastWeatherResult.Daily.Length; i++)
                {
                    Microsoft.UI.Xaml.Thickness stackPanelThickness;
                    if (i == 0)
                    {
                        stackPanelThickness = new Microsoft.UI.Xaml.Thickness();
                    }
                    else
                    {
                        stackPanelThickness = new Microsoft.UI.Xaml.Thickness(30, 0, 0, 0);
                    }

                    StackPanel dailyContent = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = stackPanelThickness
                    };

                    string imgSource = $"ms-appx:///Assets/Seniverse/Icon/{qweatherToSeniverseIcon[dailyforecastWeatherResult.Daily[i].IconDay]}@1x.png";
                    Image weatherImage = new Image
                    {
                        Source = new BitmapImage(new Uri(imgSource)),
                        Width = 50,
                        Height = 50
                    };
                    TextBlock weatherDescriptionTextBlock = new TextBlock
                    {
                        Text = $"{dailyforecastWeatherResult.Daily[i].TempMin}-{dailyforecastWeatherResult.Daily[i].TempMax}{tempUnit}",
                        FontWeight = FontWeights.SemiBold,
                        HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                        Margin = new Microsoft.UI.Xaml.Thickness(0, 5, 0, 0)
                    };
                    TextBlock dateTimeTextBlock = new TextBlock
                    {
                        Text = dailyforecastWeatherResult.Daily[i].FxDate,
                        FontWeight = FontWeights.SemiBold,
                        HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                        Margin = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 0)
                    };

                    dailyContent.Children.Add(weatherImage);
                    dailyContent.Children.Add(dateTimeTextBlock);
                    dailyContent.Children.Add(weatherDescriptionTextBlock);

                    DailyForecastStackPanel.Children.Add(dailyContent);
                }

                var warningResult = await QWeatherAPI.WeatherWarningAPI.GetWeatherWarningAsync(location.Lon, location.Lat, Configs.ApiKey);
                WeatherWarningStackPanel.Children.Clear();
                if (warningResult.Warning.Length == 0)
                {
                    WeatherWarningBorder.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
                }
                else
                {
                    WeatherWarningBorder.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                }
                for (int i = 0; i < warningResult.Warning.Length; i++)
                {
                    Microsoft.UI.Xaml.Thickness stackPanelThickness;
                    if (i == 0)
                    {
                        stackPanelThickness = new Microsoft.UI.Xaml.Thickness();
                    }
                    else
                    {
                        stackPanelThickness = new Microsoft.UI.Xaml.Thickness(30, 0, 0, 0);
                    }

                    StackPanel warningContent = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        Margin = stackPanelThickness
                    };
                    Dictionary<string, string> typeToIconType = new Dictionary<string, string>();
                    typeToIconType.Add("红色", "1");
                    typeToIconType.Add("橙色", "2");
                    typeToIconType.Add("黄色", "3");
                    typeToIconType.Add("蓝色", "4");
                    typeToIconType.Add("白色", "5");
                    string type = warningResult.Warning[i].Type;
                    if (type.Substring(0, 2) == "10")
                    {
                        string typeInt = type[2..];
                        type = "11B" + typeInt + typeToIconType[warningResult.Warning[i].Level];
                    }
                    string imgSource = $"https://a.hecdn.net/img/common/alarm/1.0/{type}.png";
                    Image warningImage = new Image
                    {
                        Source = new BitmapImage(new Uri(imgSource)),
                        Width = 50,
                        Height = 50
                    };
                    TextBlock warningTextBlock = new TextBlock
                    {
                        Text = $"{warningResult.Warning[i].TypeName}{warningResult.Warning[i].Level}预警",
                        FontWeight = FontWeights.SemiBold,
                        HorizontalTextAlignment = Microsoft.UI.Xaml.TextAlignment.Center,
                        Margin = new Microsoft.UI.Xaml.Thickness(0, 10, 0, 0)
                    };
                    HyperlinkButton warningDescriptionHyperlinkButton = new HyperlinkButton
                    {
                        NavigateUri = new Uri($"https://www.qweather.com/severe-weather/{warningResult.Warning[i].ID[..9]}.html"),
                        Content = "详细信息",
                        Margin = new Microsoft.UI.Xaml.Thickness(0, 5, 0, 0)
                    };

                    warningContent.Children.Add(warningImage);
                    warningContent.Children.Add(warningTextBlock);
                    warningContent.Children.Add(warningDescriptionHyperlinkButton);

                    WeatherWarningStackPanel.Children.Add(warningContent);
                }

                queryLocation = location;

                StopLoading();
            }
            catch
            {
                StopLoading();

                SearchLocationTipTextBlock.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
                SearchLocationTipTextBlock.Opacity = 1;

                WeatherInfoFrame.Opacity = 0;
                WeatherInfoFrame.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void RefreshButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (queryLocation == null)
            {
                return;
            }

            QueryWeatherAsync(queryLocation);
        }
    }
}