using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.Tools.Net.Http
{
    internal static class WebRequests
    {
        // Get 请求
        internal static async Task<HttpResponseMessage> GetRequestAsync(string Url, HttpMessageHandler handler = null)
        {
            Url = Url.Trim();
            if (handler == null)
            {
                handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                };
            }
            using (HttpClient client = new HttpClient(handler))
            {
                return await client.GetAsync(Url);
            }
        }

        // 下载文件
        internal static async Task DownloadFile(string url, string downloadPath, string fileName, IWebProxy proxy = null)
        {
            HttpMessageHandler handler = new HttpClientHandler()
            {
                Proxy = proxy
            };
            using (var response = await GetRequestAsync(url, handler))
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    if (!Directory.Exists(downloadPath))
                    {
                        Directory.CreateDirectory(downloadPath);
                    }
                    if (File.Exists(Path.Combine(downloadPath, fileName)))
                    {
                        File.Delete(Path.Combine(downloadPath, fileName));
                    }
                    string extension = Path.GetExtension(response.RequestMessage.RequestUri.ToString());
                    using (FileStream fs = new FileStream(Path.Combine(downloadPath, fileName), FileMode.CreateNew))
                    {
                        byte[] buffer = new byte[1024 * 1024 * 8];
                        int length;
                        while ((length = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                        {
                            await fs.WriteAsync(buffer, 0, length);
                        }
                    }
                }
            }
        }
    }
}
