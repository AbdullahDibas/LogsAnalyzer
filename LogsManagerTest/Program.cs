using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LogsManager;
using LogsManager.Common.Enums;
using LogsManager.Common.Analyzer.Config;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Rules;
using LogsManager.Common.Analyzer.Outputs;

namespace LogsManagerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //string configurationFileContent = JsonConvert.SerializeObject(GetAnalyzerConfig(), new Newtonsoft.Json.Converters.StringEnumConverter());

            // specify the way you are going to use to send your logs to the analyzer.
            ILogsSender logsSender = GetLogsSender();

            // initialize the logs managers class that's going to handle incoming logs messages and send them to the analyzer
            // using the logs sender.
            LogsManager.LogsManager logsManager = new LogsManager.LogsManager(IsLogsAnalyzerEnabled, logsSender);

            logsManager.Initialize();

            // the following are some test for analyzer rules.
            Task.Delay(12000).ContinueWith((t) =>
            {
                TestFilterOnlyRule(logsManager);

                Task.Run(() => TestTimeDifferenceRule(logsManager));

                Task.Run(() => TestLogFrequencyRule(logsManager));

                Task.Run(() => TestAggregateFunctionRule(logsManager));

                Task.Run(() => TestDuplicateLogDetectionRule(logsManager));
            });            

            Console.ReadLine();
        }

        private static ILogsSender GetLogsSender()
        {
            ILogsSender logsSender = new DefaultSender(); // Or AnonymousPipesSender or NamedPipesSender if the analyzer is hosted in a separate application.

            return logsSender;
        }

        private static bool IsLogsAnalyzerEnabled()
        {
            return bool.TryParse(ConfigurationManager.AppSettings["IsLogsAnalyzerEnabled"]?.ToLower(), out bool isEnabled) && isEnabled;
        }

        #region Generate Sample Analyzer Configuration

        private static AnalyzerConfig GetAnalyzerConfig()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            JsonConvert.DefaultSettings = () => settings;

            AnalyzerConfig analyzerConfig = new AnalyzerConfig
            {
                LogsReceiverConfig = new LogsReceiverConfig { LogReceiverType = LogReceiverTypes.None },

                LogMessages = GetLogMessageConfigs(),

                IsEnabled = true,

                Outputs = GetAnalyzerOutputConfigs(),

                Rules = GetAnalyzerRuleConfigs(),

                Schedules = GetAnalyzerScheduleConfigs(),

                Conditions = GetAnalyzerConditionConfigs(),

                RulesOutputs = new Dictionary<int, int> { { 1, 1 }, { 2, 2 }, { 3, 3 }, { 4, 4 }, { 5, 4 } },

                RulesSchedules = new Dictionary<int, int> { { 1, 2 }, { 2, 2 }, { 3, 1 }, { 4, 1 }, { 5, 1 } }
            };

            return analyzerConfig;
        }

        private static LogMessageConfig[] GetLogMessageConfigs()
        {
            LogMessageConfig[] LogMessages = new LogMessageConfig[6];

            LogMessages[0] = new LogMessageConfig
            {
                ID = 1,
                LogMessageParameters = new LogMessageParameter[]
                {
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Level,
                        FilterValue = LogLevels.Warn.ToString()
                    }
                    ,
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Message,
                        FilterValue = "10.0.0.132",
                        TextCompareType = TextCompareTypes.Contains
                    }
                }
            };
            LogMessages[1] = new LogMessageConfig
            {
                ID = 2,
                LogMessageParameters = new LogMessageParameter[]
                {
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Level,
                        FilterValue = LogLevels.Info.ToString()
                    }
                    ,
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Message,
                        FilterValue = "10.0.0.154",
                        TextCompareType = TextCompareTypes.Contains
                    }
                }
            };
            LogMessages[2] = new LogMessageConfig
            {
                ID = 3,
                LogMessageParameters = new LogMessageParameter[]
                {
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Level,
                        FilterValue = LogLevels.Debug.ToString()
                    }
                    ,
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Message,
                        FilterValue = "10.0.0.196",
                        TextCompareType = TextCompareTypes.Contains
                    }
                }
            };
            LogMessages[3] = new LogMessageConfig
            {
                ID = 4,
                LogMessageParameters = new LogMessageParameter[]
               {
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Level,
                        FilterValue = LogLevels.Info.ToString()
                    }
                    ,
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Message,
                        FilterValue = "10.0.0.82",
                        TextCompareType = TextCompareTypes.Contains
                    }
               }
            };
            LogMessages[4] = new LogMessageConfig
            {
                ID = 5,
                LogMessageParameters = new LogMessageParameter[]
                {
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Tags,
                        FilterValue = "Clothes"
                    }
                }
            };
            LogMessages[5] = new LogMessageConfig
            {
                ID = 6,
                LogMessageParameters = new LogMessageParameter[]
                {
                    new LogMessageParameter
                    {
                        FilterParameter = LogMessageParameters.Message,
                        FilterValue = "New payment",
                        TextCompareType = TextCompareTypes.Contains
                    }
                }
            };

            return LogMessages;
        }

        private static AnalyzerOutputConfig[] GetAnalyzerOutputConfigs()
        {
            return new AnalyzerOutputConfig[]
                {
                    new FileOutputConfig
                    {
                        ID = 1,
                        OutputType = OutputTypes.File,
                        FilePath = "C:\\logs_1.txt",
                        IncludedMessageParameters = new LogMessageParameters[] { LogMessageParameters.DateTime , LogMessageParameters.Message }
                    },
                    new FileOutputConfig
                    {
                        ID = 2,
                        OutputType = OutputTypes.File,
                        FilePath = "C:\\logs_2.txt",
                        IncludedMessageParameters = new LogMessageParameters[] { LogMessageParameters.Level , LogMessageParameters.Tags }
                    },
                    new FileOutputConfig
                    {
                        ID = 3,
                        OutputType = OutputTypes.File,
                        FilePath = "C:\\logs_3.txt",
                        IncludedMessageParameters = new LogMessageParameters[] { LogMessageParameters.Level , LogMessageParameters.Tags }
                    },
                    new FileOutputConfig
                    {
                        ID = 4,
                        OutputType = OutputTypes.CSVFile,
                        FilePath = "C:\\logs_4.csv",
                        IncludedMessageParameters = new LogMessageParameters[] { LogMessageParameters.DateTime, LogMessageParameters.Level , LogMessageParameters.Message}
                    },
                    new AnalyzerOutputConfig
                    {
                        ID = 5,
                        OutputType = OutputTypes.WindowsNotification,
                        IncludedMessageParameters = new LogMessageParameters[] { LogMessageParameters.DateTime, LogMessageParameters.Level , LogMessageParameters.Message}
                    }
                };
        }

        private static AnalyzerRuleConfig[] GetAnalyzerRuleConfigs()
        {
           return new AnalyzerRuleConfig[]
               {
                new  LogFilterRule {ID = 1, IsEnabled = true,  LogMessagesIDs = new int[] { 1 } },
                new TimeDifferenceRule
                {
                    IsEnabled = true,
                    ID = 2,
                    AnalysisType = LogAnalysisTypes.TimeDifference ,
                    FirstLogMessageID = 2,
                    SecondLogMessageID = 3
                },
                new LogFrequencyRule
                {
                    IsEnabled = true,
                    ID = 3,
                    AnalysisType = LogAnalysisTypes.LogFrequency ,
                    LogMessageID = 4,
                    TimeInSeconds = 5
                },
                new AggregateFunctionRule
                {
                    IsEnabled = true,
                    ID = 4,
                    AnalysisType = LogAnalysisTypes.AggregateFunction ,
                    LogMessageID = 5,
                    ParamName = "Amount",
                    AggregateFunction = AggregateFunctions.Sum,
                    ConditionsIDs = new int[]{ 1 }
                },
                new DuplicateDetectionRule
                {
                    IsEnabled = true,
                    ID = 5,
                    AnalysisType = LogAnalysisTypes.DuplicateDetection,
                    LogMessageID = 6,
                    MaximumNumberOfMonitoredMessages = 10
                }
               };
        }

        private static AnalyzerScheduleConfig[] GetAnalyzerScheduleConfigs()
        {
           return new AnalyzerScheduleConfig[]
               {
                new AnalyzerScheduleConfig
                {
                    ID = 1,
                    ScheduleType = ScheduleTypes.Always
                },
                new AnalyzerScheduleConfig
                {
                    ID = 2,
                    ScheduleType = ScheduleTypes.OneTimeOnly,
                    StartTimeSpan = new TimeSpan(14, 0,0),
                    EndTimeSpan = new TimeSpan(15,0,0)
                }
               };
        }

        private static AnalyzerConditionConfig[] GetAnalyzerConditionConfigs()
        {
            return new AnalyzerConditionConfig[]
            {
                new AnalyzerConditionConfig
                {
                    ID = 1,
                    ParamName = "Value",
                    CompareType = ValueCompareTypes.HigherThan,
                    Value = 100
                }
            };
        }

        #endregion

        #region Test Methods

        static void TestFilterOnlyRule(LogsManager.LogsManager logsManager)
        {
            for (int i = 0; i < 5000; i++)
            {
                logsManager.Warn("10.0.0.132");
                logsManager.Info("New payment");
            }
        }

        static void TestTimeDifferenceRule(LogsManager.LogsManager logsManager)
        {
            logsManager.Info("10.0.0.154");

            logsManager.Warn("10.0.0.154");
            logsManager.Warn("10.0.0.132");
            logsManager.Warn("10.0.0.er");
            logsManager.Warn("10.0.0.er");
            logsManager.Debug("10.0.0.196");

            logsManager.Info("10.0.0.154");

            logsManager.Warn("10.0.0.154");
            logsManager.Warn("10.0.0.132");
            System.Threading.Thread.Sleep(100);
            logsManager.Warn("10.0.0.er");
            logsManager.Warn("10.0.0.er");
            logsManager.Debug("10.0.0.196");

            logsManager.Info("10.0.0.154");

            logsManager.Warn("10.0.0.154");
            logsManager.Warn("10.0.0.132");
            logsManager.Warn("10.0.0.er");
            logsManager.Warn("10.0.0.er");
        }

        static void TestLogFrequencyRule(LogsManager.LogsManager logsManager)
        {
            logsManager.Info("10.0.0.82");
            logsManager.Info("10.0.0.82");
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            logsManager.Info("10.0.0.82");
            logsManager.Info("10.0.0.82");
            logsManager.Info("10.0.0.82");
            System.Threading.Thread.Sleep(500);
            logsManager.Info("10.0.0.82");
            logsManager.Info("10.0.0.82");
            logsManager.Info("10.0.0.82");
            System.Threading.Thread.Sleep(500);
            logsManager.Warn("10.0.0.82");
            logsManager.Warn("10.0.0.82");
            logsManager.Warn("10.0.0.82");
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            logsManager.Warn("10.0.0.82");
            logsManager.Warn("10.0.0.82");
            logsManager.Info("10.0.0.79");
            System.Threading.Thread.Sleep(500);
            logsManager.Warn("10.0.0.82");
            logsManager.Warn("10.0.0.82");
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            System.Threading.Thread.Sleep(500);
            logsManager.Warn("10.0.0.82");
            logsManager.Warn("10.0.0.82");
        }

        static void TestAggregateFunctionRule(LogsManager.LogsManager logsManager)
        {
            logsManager.Info("New credit card payment", tags: new string[] { "Clothes", "T-shirt" }, parameters: new KeyValuePair<string, string>("Amount", "30") );
            logsManager.Info("New cash payment", tags: new string[] { "Clothes", "Shoes" }, parameters: new KeyValuePair<string, string>("Amount", "40") );
            logsManager.Info("New credit card payment", tags: new string[] { "Clothes", "Jacket" }, parameters: new KeyValuePair<string, string>("Amount", "90") );
            logsManager.Info("Gift card", tags: new string[] { "Clothes", "Gloves" }, parameters: new KeyValuePair<string, string>("Amount", "10") );
            logsManager.Info("New credit card payment", tags: new string[] { "Clothes", "T-shirt" }, parameters: new KeyValuePair<string, string>("Amount", "5") );
        }

        static void TestDuplicateLogDetectionRule(LogsManager.LogsManager logsManager)
        {
            logsManager.Info("New payment submitted by John for the amount 100");
            System.Threading.Thread.Sleep(3000);
            logsManager.Info("New payment submitted by John for the amount 30");
            logsManager.Info("New payment submitted by John for the amount 100");
        }

        #endregion
    }
}
