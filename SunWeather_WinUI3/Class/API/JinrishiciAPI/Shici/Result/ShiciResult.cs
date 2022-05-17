using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.JinrishiciAPI.Shici.Result
{
    internal struct ShiciResult
    {
        internal string Status;
        internal Data Data;
        internal string Token;
        internal string IpAddress;

        internal ShiciResult(string jsonString)
        {
            JObject jsonData = JObject.Parse(jsonString);
            Status = jsonData.Value<string>("status");
            Data = new Data(jsonData.SelectToken("data"));
            Token = jsonData.Value<string>("token");
            IpAddress = jsonData.Value<string>("ipAddress");
        }

        public static implicit operator string(ShiciResult shici)
        {
            return shici.Data.Content;
        }
    }
}
