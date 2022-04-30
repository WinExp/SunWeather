using Newtonsoft.Json.Linq;
using SunWeather_WinUI3.Class.Tools.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.Ipify
{
    internal static class GetIP
    {
        internal static async Task<string> GetIPAsync()
        {
            var response = await WebRequests.GetRequestAsync("https://api.ipify.org/?format=json", 15000);

            using (var stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    JObject jsonData = JObject.Parse(reader.ReadToEnd());
                    return jsonData.Value<string>("ip");
                }
            }
        }
    }
}
