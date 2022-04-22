using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunWeather_WinUI3.Class.Tools.Config
{
    internal class ConfigWriter : IDisposable
    {
        private string filePath;
        private Dictionary<string, string> keyValues = new Dictionary<string, string>();
        private bool disposedValue;

        internal ConfigWriter(string filePath)
        {
            this.filePath = filePath;
            if (File.Exists(filePath))
            {
                ReadConfig();
            }
        }

        // 读取配置
        private void ReadConfig()
        {
            lock (this)
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string key = line.Substring(0, line.IndexOf('='));
                        string value = line.Substring(line.IndexOf('=') + 1);
                        keyValues.Add(key, value);
                    }
                }
            }
        }

        // 设定配置值
        internal void SetConfigValue(string key, string value)
        {
            if (key.Contains('='))
            {
                throw new ArgumentException("键内不允许包含等号。");
            }
            if (keyValues.ContainsKey(key))
            {
                keyValues[key] = value;
                return;
            }
            keyValues.Add(key, value);
        }

        // 删除配置值
        internal void DelConfigValue(string key)
        {
            keyValues.Remove(key);
        }

        // 写入文件
        internal void WriteInFile()
        {
            lock (keyValues)
            {
                string directoryName = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in keyValues)
                    {
                        string line = $"{item.Key}={item.Value}";
                        writer.WriteLine(line);
                    }
                }
            }
        }

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
        // ~ConfigWriter()
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
