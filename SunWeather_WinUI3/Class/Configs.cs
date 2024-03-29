﻿using SunWeather_WinUI3.Class.Tools.Config;
using System.IO;

namespace SunWeather_WinUI3.Class
{
    internal static class Configs
    {
        internal const string ConfigFilePath = "Sun Weather\\Config.ini";
        internal const string DefaultApiKey = "847bb1b80e9a417681dd061594197e6c";

        internal static QWeatherAPI.Tools.Units Unit;
        internal static string ApiKey;
        internal static int AutoRefreshDelay;
        internal static bool IsAutoUpdate;
        internal static bool IsTray;
        internal static bool IsToastRain;
        internal static bool IsToastWarning;

        internal static void WriteConfig()
        {
            using (ConfigWriter writer = new ConfigWriter(ConfigFilePath))
            {
                writer.SetConfigValue("Unit", Unit.ToString());
                writer.SetConfigValue("ApiKey", ApiKey == DefaultApiKey ? "" : ApiKey);
                writer.SetConfigValue("AutoRefreshDelay", AutoRefreshDelay.ToString());
                writer.SetConfigValue("IsAutoUpdate", IsAutoUpdate.ToString());
                writer.SetConfigValue("IsTray", IsTray.ToString());
                writer.SetConfigValue("IsToastRain", IsToastRain.ToString());
                writer.SetConfigValue("IsToastWarning", IsToastWarning.ToString());
                writer.WriteInFile();
            }
            LoadConfig();
        }

        internal static void LoadConfig()
        {
            // 写入默认配置
            void WriteDefaultConfig()
            {
                using (ConfigWriter configWriter = new ConfigWriter(ConfigFilePath))
                {
                    configWriter.SetConfigValue("Unit", "Metric");
                    configWriter.SetConfigValue("ApiKey", "");
                    configWriter.SetConfigValue("AutoRefreshDelay", "5");
                    configWriter.SetConfigValue("IsAutoUpdate", "True");
                    configWriter.SetConfigValue("IsTray", "True");
                    configWriter.SetConfigValue("IsToastRain", "True");
                    configWriter.SetConfigValue("IsToastWarning", "True");
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

                    string key = configLoader.GetValue("ApiKey");
                    ApiKey = key == "" ? DefaultApiKey : key;

                    int delay = int.Parse(configLoader.GetValue("AutoRefreshDelay"));
                    if (delay > 60 || delay <= 0 && delay != -1)
                    {
                        throw new FileLoadException();
                    }
                    AutoRefreshDelay = delay;

                    IsAutoUpdate = bool.Parse(configLoader.GetValue("IsAutoUpdate"));
                    IsTray = bool.Parse(configLoader.GetValue("IsTray"));
                    IsToastRain = bool.Parse(configLoader.GetValue("IsToastRain"));
                    IsToastWarning = bool.Parse(configLoader.GetValue("IsToastWarning"));
                }
            }
            catch
            {
                try
                {
                    WriteDefaultConfig();
                    LoadConfig();
                }
                catch
                {
                    Unit = QWeatherAPI.Tools.Units.Metric;
                    ApiKey = DefaultApiKey;
                    AutoRefreshDelay = 5;
                    IsAutoUpdate = true;
                    IsTray = true;
                    IsToastRain = true;
                    IsToastWarning = true;
                }
            }
        }
    }
}
