using SunWeather_WinUI3.Class.API.JinrishiciAPI.Token.Result;
using SunWeather_WinUI3.Class.Tools.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.JinrishiciAPI.Token
{
    internal static class TokenAPI
    {
        internal static async Task<TokenResult> QueryTokenAsync()
        {
            using (var response = await WebRequests.GetRequestAsync("https://v2.jinrishici.com/token"))
            {
                using (var stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return new TokenResult(reader.ReadToEnd());
                    }
                }
            }
        }
    }
}
