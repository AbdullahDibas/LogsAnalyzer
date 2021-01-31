using LogsManager.Common.Analyzer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LogsManager.Analyzer.Factories
{
    internal static class ConfigurationsFactory
    {
        /// <summary>
        /// the path of the configuration file. 
        /// </summary>
        private static string _configFilePath = "LogsAnalyzer.json";

        public static AnalyzerConfig GetAnalyzerConfig()
        {
            AnalyzerConfig analyzerConfig = null;

            if (File.Exists(_configFilePath))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                JsonConvert.DefaultSettings = () => settings;

                string settingss = File.ReadAllText(_configFilePath);

                analyzerConfig = JsonConvert.DeserializeObject<AnalyzerConfig>(settingss, new Newtonsoft.Json.Converters.StringEnumConverter());
            }

            return analyzerConfig;
        }
    }
}
