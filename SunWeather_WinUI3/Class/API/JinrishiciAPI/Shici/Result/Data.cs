using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.JinrishiciAPI.Shici.Result
{
    internal struct Data
    {
        internal string Id;
        internal string Content;
        internal string Popularity;
        internal Origin Origin;
        internal string[] MatchTags;
        internal string RecommendedReason;
        internal string CacheAt;

        internal Data(JToken token)
        {
            Id = token.Value<string>("id");
            Content = token.Value<string>("content");
            Popularity = token.Value<string>("popularity");
            Origin = new Origin(token.SelectToken("origin"));
            List<string> tags = new List<string>();
            foreach (var tag in token.Value<JArray>("matchTags"))
            {
                tags.Add(tag.ToString());
            }
            MatchTags = tags.ToArray();
            RecommendedReason = token.Value<string>("recommendedReason");
            CacheAt = token.Value<string>("cacheAt");
        }
    }
}
