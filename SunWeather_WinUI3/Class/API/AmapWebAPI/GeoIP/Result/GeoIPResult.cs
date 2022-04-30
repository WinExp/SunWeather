using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.AmapWebAPI.GeoIP.Result
{
    internal struct GeoIPResult
    {
        internal string Status;
        internal string Info;
        internal string InfoCode;
        internal string Province;
        internal string City;
        internal string AdCode;
        internal string Rectangle;

        internal GeoIPResult(string jsonString)
        {
            JObject jsonData = JObject.Parse(jsonString);

            Status = jsonData.Value<string>("status");
            Info = jsonData.Value<string>("info");
            InfoCode = jsonData.Value<string>("infocode");
            Province = jsonData.Value<string>("province");
            City = jsonData.Value<string>("city");
            AdCode = jsonData.Value<string>("adcode");
            Rectangle = jsonData.Value<string>("rectangle");
        }
    }
}
