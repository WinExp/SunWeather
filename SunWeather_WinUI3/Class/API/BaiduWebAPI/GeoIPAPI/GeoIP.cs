using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.BaiduWebAPI.GeoIPAPI
{
    internal static class GeoIP
    {
        internal async static Task<Result.GeoIPResult> QueryGeoIPAsync(string ak, Coor coor = Coor.Bd09ll)
        {
            string coorStr;
            switch (coor)
            {
                case Coor.Bd09ll:
                    coorStr = "bd09ll";
                    break;
                case Coor.Gcj02:
                    coorStr = "gcj02";
                    break;
                default:
                    goto case Coor.Bd09ll;
            }

            using (var response = await Tools.Net.Http.WebRequests.GetRequestAsync($"https://api.map.baidu.com/location/ip?ak={ak}&coor={coorStr}"))
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return new Result.GeoIPResult(await reader.ReadToEndAsync());
                    }
                }
            }
        }

        internal async static Task<Result.GeoIPResult> QueryGeoIPAsync(string ak, string ip, Coor coor = Coor.Bd09ll)
        {
            string coorStr;
            switch (coor)
            {
                case Coor.Bd09ll:
                    coorStr = "bd09ll";
                    break;
                case Coor.Gcj02:
                    coorStr = "gcj02";
                    break;
                default:
                    goto case Coor.Bd09ll;
            }

            using (var response = await Tools.Net.Http.WebRequests.GetRequestAsync($"https://api.map.baidu.com/location/ip?ak={ak}&ip={ip}&coor={coorStr}"))
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return new Result.GeoIPResult(await reader.ReadToEndAsync());
                    }
                }
            }
        }

        internal enum Coor
        {
            Bd09ll,
            Gcj02
        }
    }
}
