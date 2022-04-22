using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.Tools.Config
{
    internal class ConfigLoader : IDisposable
    {
        private string filePath;
        private Dictionary<string, string> keyValues = new Dictionary<string, string>();
        private bool disposedValue;

        internal ConfigLoader(string filePath)
        {
            this.filePath = filePath;
            ReadConfig();
        }

        // 读取配置
        private void ReadConfig()
        {
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string key = line.Substring(0, line.IndexOf('='));
                    string value;
                    try
                    {
                        value = line.Substring(line.IndexOf('=') + 1);
                        keyValues.Add(key, value);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        using (ConfigWriter writer = new ConfigWriter(filePath))
                        {
                            writer.DelConfigValue(key);
                        }
                    }
                }
            }
        }

        // 获取配置值
        internal string GetValue(string key)
        {
            string result = keyValues[key];
            return result;
        }

        internal void TryGetValue(string key, out string result)
        {
            keyValues.TryGetValue(key, out result);
        }

        // IDisposable 实现
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    filePath = null;
                    keyValues.Clear();
                    keyValues = null;
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ConfigLoader()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
