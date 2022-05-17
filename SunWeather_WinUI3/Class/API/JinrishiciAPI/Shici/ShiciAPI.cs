using SunWeather_WinUI3.Class.API.JinrishiciAPI.Shici.Result;
using SunWeather_WinUI3.Class.Tools.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.JinrishiciAPI.Shici
{
    internal static class ShiciAPI
    {
        internal static async Task<ShiciResult> QueryShiciAPIAsync(string token)
        {
            var headers = new WebHeaderCollection();
            headers.Add($"X-User-Token:{token}");
            using (var response = await WebRequests.GetRequestAsync("https://v2.jinrishici.com/sentence", headers))
            {
                using (var stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return new ShiciResult(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
