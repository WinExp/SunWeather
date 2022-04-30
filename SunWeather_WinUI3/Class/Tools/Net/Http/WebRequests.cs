using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.Tools.Net.Http
{
    internal static class WebRequests
    {
        // Get 请求
        internal static async Task<WebResponse> GetRequestAsync(string url, HttpWebRequest request = null)
        {
            url = url.Trim();

            if (request == null)
            {
                request = WebRequest.CreateHttp(url);
                request.Method = "GET";
                request.AutomaticDecompression = DecompressionMethods.GZip;
                request.Timeout = 3000;
                request.KeepAlive = false;
                request.Proxy = null;
            }
            
            var response = await request.GetResponseAsync();
            return response;
        }

        internal static async Task<WebResponse> GetRequestAsync(string url, int timeout)
        {
            url = url.Trim();

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Timeout = timeout;
            request.KeepAlive = false;
            request.Proxy = null;

            var response = await request.GetResponseAsync();
            return response;
        }
    }
}
