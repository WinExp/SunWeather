using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.BaiduWebAPI.GeoIPAPI.Result
{
    internal struct Content
    {
        internal string Address;
        internal Address_Detail Address_Detail;
        internal Point Point;

        internal Content(JToken token)
        {
            Address = token.Value<string>("address");
            Address_Detail = new Address_Detail(token.SelectToken("address_detail"));
            Point = new Point(token.SelectToken("point"));
        }
    }

    internal struct Address_Detail
    {
        internal string City;
        internal string City_Code;
        internal string Province;

        internal Address_Detail(JToken token)
        {
            City = token.Value<string>("city");
            City_Code = token.Value<string>("city_code");
            Province = token.Value<string>("province");
        }
    }

    internal struct Point
    {
        internal double X;
        internal double Y;

        internal Point(JToken token)
        {
            X = token.Value<double>("x");
            Y = token.Value<double>("y");
        }
    }
}
