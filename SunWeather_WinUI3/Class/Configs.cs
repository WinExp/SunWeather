using SunWeather_WinUI3.Class.Tools.Config;
using System.IO;

namespace SunWeather_WinUI3.Class
{
    internal static class Configs
    {
        internal const string ConfigFilePath = @"Sun Weather\Config.ini";
        internal static QWeatherAPI.Tools.Units Unit;
        internal static bool PreLoadAPI;
        internal static string ApiKey;
        internal const string DefaultApiKey = "847bb1b80e9a417681dd061594197e6c";

        // 读取配置文件
        internal static void LoadConfig()
        {
            // 写入默认配置
            void WriteDefaultConfig()
            {
                using (ConfigWriter configWriter = new ConfigWriter(ConfigFilePath))
                {
                    configWriter.SetConfigValue("Unit", "Metric");
                    configWriter.SetConfigValue("PreLoadAPI", "True");
                    configWriter.SetConfigValue("ApiKey", "");
                    configWriter.WriteInFile();
                }
            }

            if (!File.Exists(ConfigFilePath))
            {
                WriteDefaultConfig();
            }

            try
            {
                using (ConfigLoader configLoader = new ConfigLoader(ConfigFilePath))
                {
                    string unitValue = configLoader.GetValue("Unit");
                    switch (unitValue)
                    {
                        case "Metric":
                            Unit = QWeatherAPI.Tools.Units.Metric;
                            break;
                        case "Inch":
                            Unit = QWeatherAPI.Tools.Units.Inch;
                            break;
                        default:
                            throw new FileLoadException();
                    }

                    string preLoadValue = configLoader.GetValue("PreLoadAPI");
                    switch (preLoadValue)
                    {
                        case "True":
                            PreLoadAPI = true;
                            break;
                        case "False":
                            PreLoadAPI = false;
                            break;
                        default:
                            throw new FileLoadException();
                    }

                    string key = configLoader.GetValue("ApiKey");
                    ApiKey = key == "" ? DefaultApiKey : key;
                }
            }
            catch
            {
                try
                {
                    WriteDefaultConfig();
                    LoadConfig();
                }
                catch { }
                Unit = QWeatherAPI.Tools.Units.Metric;
                PreLoadAPI = true;
                ApiKey = DefaultApiKey;
            }
        }
    }
}
