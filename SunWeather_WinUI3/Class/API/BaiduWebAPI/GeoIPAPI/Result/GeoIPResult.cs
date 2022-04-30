using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.BaiduWebAPI.GeoIPAPI.Result
{
    internal struct GeoIPResult
    {
        internal string Address;
        internal Content Content;
        internal string Status;

        internal GeoIPResult(string jsonString)
        {
            JObject jsonData = JObject.Parse(jsonString);

            Address = jsonData.Value<string>("address");
            Content = new Content(jsonData.SelectToken("content"));
            Status = jsonData.Value<string>("status");
        }
    }
}
