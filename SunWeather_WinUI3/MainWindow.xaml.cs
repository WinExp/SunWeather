﻿using EasyUpdate;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using QWeatherAPI.Result.GeoAPI.CityLookup;
using SunWeather_WinUI3.Class;
using SunWeather_WinUI3.Class.Model.Command.Base;
using SunWeather_WinUI3.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SunWeather_WinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowEx
    {
        private Dictionary<string, Location> locationDictionary = new Dictionary<string, Location>();
        private ObservableCollection<string> locationName = new ObservableCollection<string>();
        private const string updateInfoUrl = "https://we-bucket.oss-cn-shenzhen.aliyuncs.com/Project/Download/SunWeather/Update/updateInfo.xml";
        private bool isUpdateChecked = false;
        private bool isTrayExit = false;
        private bool isFirstClose = true;
        internal static bool IsBackground = false;
        internal static HomePage homePage;
        internal static SearchPage searchPage;

        public MainWindow()
        {
            // 初始化窗口
            this.InitializeComponent();
            SearchPage.mainWindow = this;
            SettingPage.mainWindow = this;
            this.SetIcon("Assets/App_Icon_Content_64.ico");
            this.SearchLocationAutoSuggestBox.ItemsSource = locationName;
            Configs.LoadConfig();
            this.Activated += async delegate
            {
                if (MicaController.IsSupported()) this.Backdrop = new MicaSystemBackdrop();
                if (Configs.IsAutoUpdate && !isUpdateChecked)
                {
                    isUpdateChecked = true;
                    try
                    {
                        await CheckUpdateAsync();
                    }
                    catch { }
                }
            };
            ToastNotificationManagerCompat.History.Clear();
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);
                if (args.Contains("isTray") && bool.Parse(args["isTray"]))
                {
                    return;
                }
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    this.Show();
                    this.BringToFront();
                    HomePageItem.IsSelected = true;
                    homePage.ToastCallback();
                });
            };
            LoadTray();
        }

        private void LoadTray()
        {
            var notifyIcon = NotifyIcon.Create();
            string version = Assembly.GetEntryAssembly().GetName().Version.ToString();
            notifyIcon.Text = $@"Sun Weather
Release {version.Remove(version.LastIndexOf('.'))}";
            notifyIcon.Icon = "Assets\\App_Icon_Content_64.ico";
            notifyIcon.Click += delegate
            {
                this.Show();
                this.BringToFront();
                IsBackground = false;
            };
            notifyIcon.ContextMenuStrip.Items.Add(new ContextMenuStrip.MenuItem
            {
                Text = "退出应用",
                Command = ExitCommand
            });

            this.Closed += (s, e) =>
            {
                bool isDebug = false;
#if DEBUG
                isDebug = true;
#endif
                if (isTrayExit || !Configs.IsTray || isDebug)
                {
                    ToastNotificationManagerCompat.Uninstall();
                    notifyIcon.Dispose();
                }
                else
                {
                    this.Hide();
                    if (isFirstClose)
                    {
                        Task.Run(async () =>
                        {
                            new ToastContentBuilder()
                            .AddArgument("isTray", "true")
                            .AddText("提示")
                            .AddText("Sun Weather 已最小化至托盘。")
                            .Show(toast =>
                            {
                                toast.Tag = "trayInfo";
                            });
                            await Task.Delay(5000);
                            ToastNotificationManagerCompat.History.Remove("trayInfo");
                        });
                        isFirstClose = false;
                    }
                    IsBackground = true;
                    e.Handled = true;
                }
            };
        }

        private ICommand ExitCommand
        {
            get
            {
                return new ICommandBase
                {
                    CommandAction = () =>
                    {
                        isTrayExit = true;
                        App.Current.Exit();
                    }
                };
            }
        }

        internal async Task CheckUpdateAsync()
        {
            try
            {
                var updateInfo = await Updater.GetUpdateInfoAsync(updateInfoUrl);
                if (updateInfo.IsUpdateAvailable)
                {
                    if (updateInfo.Forced == ForcedMode.Normal)
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "更新可用",
                            Content = $"Sun Weather {updateInfo.Version} 版本可用。您正在使用 {Assembly.GetEntryAssembly().GetName().Version}，请问是否要更新？",
                            PrimaryButtonText = "更新",
                            CloseButtonText = "下次再说",
                            DefaultButton = ContentDialogButton.Primary,
                            XamlRoot = this.Content.XamlRoot
                        };
                        if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
                        {
                            await StartUpdateAsync(updateInfo);
                        }
                    }
                    else
                    {
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "更新可用",
                            Content = $"Sun Weather {updateInfo.Version} 版本可用。您正在使用 {Assembly.GetEntryAssembly().GetName().Version}，你必须更新到此版本才可继续使用 Sun Weather。",
                            PrimaryButtonText = "更新",
                            DefaultButton = ContentDialogButton.Primary,
                            XamlRoot = this.Content.XamlRoot
                        };
                        if (await contentDialog.ShowAsync() == ContentDialogResult.Primary)
                        {
                            await StartUpdateAsync(updateInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ContentDialog contentDialog = new ContentDialog
                {
                    Title = "发生错误",
                    Content = $@"在更新时出现错误，错误信息：
{ex}"
                };
                await contentDialog.ShowAsync();
            }
        }

        private async Task StartUpdateAsync(UpdateInfo updateInfo)
        {
            UpdateDialogPage updateDialogPage = new UpdateDialogPage();
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "更新",
                Content = updateDialogPage,
                XamlRoot = this.Content.XamlRoot
            };
            contentDialog.ShowAsync();
            await updateDialogPage.UpdateAppAsync(updateInfo);
        }

        private async Task<bool> CheckApiKeyAsync(bool showDialog = true)
        {
            async Task ShowContentDialogAsync(string errorCode)
            {
                if (showDialog)
                {
                    ContentDialog contentDialog = new ContentDialog
                    {
                        Title = "提示",
                        Content = $@"请到设置页面配置正确的 API 密钥。
错误代码：{errorCode}",
                        PrimaryButtonText = "去配置",
                        PrimaryButtonCommand = OpenSettingPageCommand,
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.Primary,
                        XamlRoot = this.Content.XamlRoot
                    };
                    await contentDialog.ShowAsync();
                }
            }
            if (string.IsNullOrEmpty(Configs.ApiKey))
            {
                await ShowContentDialogAsync("400");
                return false;
            }
            try
            {
                await QWeatherAPI.GeoAPI.GetGeoAsync("北京", Configs.ApiKey);
            }
            catch (ArgumentException ex)
            {
                await ShowContentDialogAsync(ex.Message);
                return false;
            }
            return true;
        }

        private ICommand OpenSettingPageCommand
        {
            get
            {
                return new ICommandBase
                {
                    CommandAction = () =>
                    {
                        PageContentFrame.Navigate(typeof(SettingPage));
                        MainNavigationView.IsPaneOpen = false;
                    }
                };
            }
        }

        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Type navigationPage = typeof(HomePage);
            NavigationViewItem navigationItem = (NavigationViewItem)sender.SelectedItem;
            if (navigationItem == HomePageItem)
            {
                navigationPage = typeof(HomePage);
            }
            else if (navigationItem == HelpPageItem)
            {
                navigationPage = typeof(HelpPage);
            }
            else if (navigationItem == MorePageItem)
            {
                navigationPage = typeof(MorePage);
            }
            else if (args.IsSettingsSelected)
            {
                navigationPage = typeof(SettingPage);
            }
            this.PageContentFrame.Navigate(navigationPage, null, args.RecommendedNavigationTransitionInfo);
        }

        internal async void SearchPage_LocationListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            HomePage.CustomLocation = true;
            this.PageContentFrame.Navigate(typeof(HomePage));
            await homePage.QueryWeatherAsync(searchPage.locationDictionary[listView.SelectedItem.ToString()]);
            HomePage.CustomLocation = false;
        }

        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (sender.Text == String.Empty || args.ChosenSuggestion != null || !await CheckApiKeyAsync())
            {
                return;
            }
            this.MainNavigationView.IsPaneOpen = false;
            this.PageContentFrame.Navigate(typeof(SearchPage), null);
            try
            {
                searchPage.QueryLocationAsync(sender.Text);
            }
            catch (ArgumentException ex)
            {
                switch (ex.Message)
                {
                    case "404":
                        HomePageItem.IsSelected = true;
                        PageContentFrame.Navigate(typeof(HomePage));
                        ContentDialog contentDialog = new ContentDialog
                        {
                            Title = "地区不存在",
                            Content = $"您输入的 {sender.Text} 地区不存在。",
                            PrimaryButtonText = "OK",
                            DefaultButton = ContentDialogButton.Primary,
                            XamlRoot = this.Content.XamlRoot
                        };
                        await contentDialog.ShowAsync();
                        break;
                    case "400":
                        goto case "404";
                }
            }
        }

        private async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            await QueryLocationWithAutoSuggestBoxAsync(sender);
        }

        private async Task QueryLocationWithAutoSuggestBoxAsync(AutoSuggestBox sender)
        {
            try
            {
                if (locationDictionary.ContainsKey(sender.Text))
                {
                    return;
                }
                locationDictionary.Clear();
                locationName.Clear();
                if (sender.Text == String.Empty
                    || !await CheckApiKeyAsync(false))
                {
                    return;
                }
                string cutLocation = sender.Text.Trim().Replace(" ", null);
                string[] replaceString = { "，", "。", "." };
                foreach (string replaceItem in replaceString)
                {
                    cutLocation.Replace(replaceItem, ",");
                }
                string[] queryLocation = cutLocation.Split(',');
                GeoResult result;
                if (queryLocation.Length == 1)
                {
                    result = await QWeatherAPI.GeoAPI.GetGeoAsync(queryLocation[queryLocation.Length - 1], Configs.ApiKey, limit: 20);
                }
                else
                {
                    result = await QWeatherAPI.GeoAPI.GetGeoAsync(queryLocation[queryLocation.Length - 1], Configs.ApiKey, adm: queryLocation[queryLocation.Length - 2], limit: 20);
                }
                foreach (Location location in result.Locations)
                {
                    string locationName = location.Name;
                    if (!(location.Adm2.Substring(0, location.Name.Length >= location.Adm2.Length ?
                        location.Adm2.Length : location.Name.Length) == location.Name))
                    {
                        locationName = locationName.Insert(0, location.Adm2 + ", ");
                    }
                    else if (!(location.Adm1.Substring(0, location.Name.Length >= location.Adm1.Length ?
                        location.Adm1.Length : location.Name.Length) == location.Name))
                    {
                        locationName = locationName.Insert(0, location.Adm1 + ", ");
                    }
                    else
                    {
                        locationName = locationName.Insert(0, location.Country + ", ");
                    }
                    locationDictionary.Add(locationName, location);
                    this.locationName.Add(locationName);
                }
            }
            catch (ArgumentException ex)
            {
                switch (ex.Message)
                {
                    case "404":
                        locationName.Clear();
                        break;
                }
            }
        }

        private async void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (!await CheckApiKeyAsync())
            {
                return;
            }
            this.MainNavigationView.SelectedItem = this.HomePageItem;
            string locationName = args.SelectedItem.ToString();
            await homePage.QueryWeatherAsync(locationDictionary[locationName]);
        }

        private async void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            if (!await CheckApiKeyAsync(false))
            {
                locationDictionary.Clear();
                locationName.Clear();
                return;
            }
            if (locationDictionary.Count == 0
                || locationDictionary.Count != locationName.Count)
            {
                await QueryLocationWithAutoSuggestBoxAsync(autoSuggestBox);
                return;
            }
            autoSuggestBox.IsSuggestionListOpen = true;
        }
    }
}
