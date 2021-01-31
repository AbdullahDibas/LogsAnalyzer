using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LogsManager.Analyzer
{
    internal class LogsAnalyzer : ILogsAnalyzer
    {
        private readonly AnalyzerConfig _analyzerConfig;

        private readonly Dictionary<int, IAnalyzerRuleHandler> _analyzerRuleHandlers;

        private readonly Dictionary<int, IAnalyzerOutputHandler> _analyzerOutputHandlers;

        private readonly Dictionary<int, IAnalyzerScheduleHandler> _analyzerScheduleHandlers;

        private BlockingCollection<LogMessage> _analyzerLogsCollection = new BlockingCollection<LogMessage>();

        public LogsAnalyzer(AnalyzerConfig analyzerConfig)
        {
            _analyzerConfig = analyzerConfig;

            _analyzerRuleHandlers = RuleHandlerFactory.CreateRulesHandlers(analyzerConfig);

            _analyzerOutputHandlers = OutputsHandlersFactory.GetOutputsHandlers(analyzerConfig);

            _analyzerScheduleHandlers = SchedulesHandlersFactory.CreateSchedulesHandlers(analyzerConfig);

            _analyzerRuleHandlers.Values.ToList().ForEach(arh => arh.OnAnalyzerResult += OnAnalyzerResult);

            StartAnalyzerProcess();
        }

        private void StartAnalyzerProcess()
        {
            Task consumerThread = Task.Factory.StartNew(() =>
            {
                while (!_analyzerLogsCollection.IsCompleted)
                {
                    if (_analyzerLogsCollection.TryTake(out LogMessage logMessage))
                    {
                        if (IsLogAnalyzingRequired())
                        {
                            _analyzerRuleHandlers.Values.ToList().ForEach(r =>
                            {
                                if (_analyzerScheduleHandlers.FirstOrDefault(ash => _analyzerConfig.RulesSchedules.ContainsKey(r.RuleID) &&
                                ash.Key == _analyzerConfig.RulesSchedules[r.RuleID]).Value?.IsValid ?? false)
                                {
                                    r.HandleLog(logMessage);
                                }
                            });
                        }
                    }
                }
            });
        }

        private void OnAnalyzerResult(object sender, AnalyzerResultEventArgs e)
        {
            if (e != null)
            {
                var outputID = _analyzerConfig.RulesOutputs[e.RuleID];

                var outputHandler = _analyzerOutputHandlers.FirstOrDefault(oh => oh.Key == outputID).Value;

                outputHandler?.Output(ParseLogMessage(e.Messages), e.AnalysisParameters);
            }
        }

        private Dictionary<LogMessageParameters, string>[] ParseLogMessage(LogMessage[] logMessages)
        {
            return logMessages?.ToList().Select((logMessage) =>              
                    new Dictionary<LogMessageParameters, string>
                    {
                        { LogMessageParameters.DateTime, DateTime.Now.ToString()},
                        { LogMessageParameters.Level, logMessage.LogLevel.ToString()},
                        { LogMessageParameters.Message, logMessage.Message},
                        { LogMessageParameters.Tags, logMessage.Tags != null? string.Join(", ", logMessage.Tags) : null},
                        { LogMessageParameters.Params, logMessage.Parameters != null? string.Join("|", logMessage.Parameters.Select(p => p.Key + ":" + p.Value)) : null}
                    }).ToArray();
        }

        public void AnalyzeLog(LogMessage logMessage)
        {
            _analyzerLogsCollection.TryAdd(logMessage);
        }

        public void AnalyzeLog(LogLevels logLevel, string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            _analyzerLogsCollection.TryAdd(new LogMessage { DateTime = DateTime.Now, Message = message, Exception = exception, LogLevel = logLevel, Tags = tags, Parameters = parameters });                  
        }

        private bool IsLogAnalyzingRequired()
        {
            return _analyzerConfig != null && _analyzerConfig.IsEnabled && _analyzerConfig.Rules.Any(ar => ar.IsEnabled);
        }

        public void Dispose()
        {
            _analyzerLogsCollection?.CompleteAdding();

            _analyzerOutputHandlers?.ToList().ForEach(aoh => aoh.Value?.Dispose());

            _analyzerOutputHandlers?.Clear();

            _analyzerRuleHandlers?.Clear();

            _analyzerScheduleHandlers?.Clear();
        }
    }
}
