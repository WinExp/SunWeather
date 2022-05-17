using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.JinrishiciAPI.Token.Result
{
    internal struct TokenResult
    {
        internal string Status;
        internal string Data;

        internal TokenResult(string jsonString)
        {
            JObject jsonData = JObject.Parse(jsonString);
            Status = jsonData.Value<string>("status");
            Data = jsonData.Value<string>("data");
        }

        public static implicit operator string(TokenResult token)
        {
            return token.Data;
        }
    }
}
