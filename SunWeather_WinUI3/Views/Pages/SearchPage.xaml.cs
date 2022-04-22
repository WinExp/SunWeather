using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using QWeatherAPI.Result.GeoAPI.CityLookup;
using SunWeather_WinUI3.Class;
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
    public sealed partial class SearchPage : Page
    {
        internal Dictionary<string, Location> locationDictionary = new Dictionary<string, Location>();
        internal static MainWindow mainWindow;

        public SearchPage()
        {
            this.InitializeComponent();
            MainWindow.searchPage = this;
            this.LocationListView.SelectionChanged += mainWindow.SearchPage_LocationListView_SelectionChanged;
        }

        internal async Task QueryLocationAsync(string locationName)
        {
            this.LocationListView.Opacity = 0;
            this.LocationListView.Visibility = Visibility.Collapsed;
            this.LocationProgressRing.IsActive = true;
            string cutLocation = locationName.Trim().Replace(" ", null);
            string[] replaceString = { "，", "。", "." };
            foreach (string replaceItem in replaceString)
            {
                cutLocation.Replace(replaceItem, ",");
            }
            string[] queryLocation = cutLocation.Split(',');
            GeoResult geoResult = queryLocation.Length > 1 ?
                    await QWeatherAPI.GeoAPI.GetGeoAsync(queryLocation[queryLocation.Length - 1], "c94fcc66ea614293953be7687537ba91", adm: queryLocation[queryLocation.Length - 2], limit: 20) :
                    await QWeatherAPI.GeoAPI.GetGeoAsync(queryLocation[queryLocation.Length - 1], "c94fcc66ea614293953be7687537ba91", limit: 20);
            locationDictionary.Clear();
            foreach (Location location in geoResult.Locations)
            {
                string _locationName = location.Name;
                if (!(location.Adm2.Substring(0, location.Name.Length >= location.Adm2.Length ?
                    location.Adm2.Length : location.Name.Length) == location.Name))
                {
                    _locationName = _locationName.Insert(0, location.Adm2 + ", ");
                }
                else if (!(location.Adm1.Substring(0, location.Name.Length >= location.Adm1.Length ?
                    location.Adm1.Length : location.Name.Length) == location.Name))
                {
                    _locationName = _locationName.Insert(0, location.Adm1 + ", ");
                }
                else
                {
                    _locationName = _locationName.Insert(0, location.Country + ", ");
                }
                locationDictionary.Add(_locationName, location);
            }
            this.LocationListView.ItemsSource = locationDictionary.Keys;
            this.LocationProgressRing.IsActive = false;
            this.LocationListView.Visibility = Visibility.Visible;
            this.LocationListView.Opacity = 1;
        }
    }
}
