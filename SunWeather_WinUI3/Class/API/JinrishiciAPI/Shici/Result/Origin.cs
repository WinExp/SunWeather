using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.API.JinrishiciAPI.Shici.Result
{
    internal struct Origin
    {
        internal string Title;
        internal string Dynasty;
        internal string Author;
        internal string[] Content;
        internal string[] Translate;

        internal Origin(JToken token)
        {
            Title = token.Value<string>("title");
            Dynasty = token.Value<string>("dynasty");
            Author = token.Value<string>("author");
            List<string> contents = new List<string>();
            foreach (var content in token.Value<JArray>("content"))
            {
                contents.Add(content.ToString());
            }
            Content = contents.ToArray();

            List<string> translates = new List<string>();
            var translateToken = token.SelectToken("translate");
            if (translateToken != null)
            {
                foreach (var translate in translateToken)
                {
                    translates.Add(translate.ToString());
                }
            }
            Translate = translates.ToArray();
        }
    }
}
