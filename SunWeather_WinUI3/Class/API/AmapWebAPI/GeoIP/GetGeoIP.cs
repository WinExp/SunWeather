using SunWeather_WinUI3.Class.Tools.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.AmapWebAPI.GeoIP
{
    internal static class GetGeoIP
    {
        internal static async Task<Result.GeoIPResult> GetGeoIPAsync(string key)
        {
            using (var response = await WebRequests.GetRequestAsync($"https://restapi.amap.com/v3/ip?key={key}", 10000))
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return new Result.GeoIPResult(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
