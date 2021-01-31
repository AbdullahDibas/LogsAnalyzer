using System;
using System.IO;
using Newtonsoft.Json;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Analyzer.Factories;

namespace LogsManager.Analyzer
{
    public class Program
    {
        private static ILogsAnalyzer _logsAnalyzer;

        private static string _configFilePath = "LogsAnalyzer.json";

        public static void Main(string[] args)
        {
            try
            {
                AnalyzerConfig analyzerConfig = GetAnalyzerConfig();

                if (analyzerConfig?.IsEnabled ?? false)
                {
                    _logsAnalyzer = new LogsAnalyzer(analyzerConfig);

                    ILogsReceiver logsReceiver = LogsReceiverFactory.Create(args);

                    if (logsReceiver != null)
                    {
                        logsReceiver.OnNewLogMessage += LogsReceiver_OnNewLogMessage;

                        logsReceiver.Start();
                    }
                } 

                Console.Write("Press enter to exit...");

                Console.ReadLine();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Error:");

                Console.WriteLine(exception?.Message);
            }
        }

        private static void LogsReceiver_OnNewLogMessage(object sender, LogMessage logMessage)
        {
            Console.WriteLine("New message is received.");

            _logsAnalyzer?.AnalyzeLog(logMessage);
        }

        private static AnalyzerConfig GetAnalyzerConfig()
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
