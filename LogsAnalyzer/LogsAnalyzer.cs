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
    public class LogsAnalyzer : ILogsAnalyzer
    {
        private readonly AnalyzerConfig _analyzerConfig;
        
        private readonly ILogsReceiver _logsReceiver;
        
        private readonly Dictionary<int, IAnalyzerRuleHandler> _ruleHandlers;

        private readonly Dictionary<int, IAnalyzerOutputHandler> _outputHandlers;

        private readonly Dictionary<int, IAnalyzerScheduleHandler> _scheduleHandlers;

        private BlockingCollection<LogMessage> _analyzerLogsCollection = new BlockingCollection<LogMessage>();

        public LogsAnalyzer(AnalyzerConfig analyzerConfig, ILogsReceiver logsReceiver,
            Dictionary<int, IAnalyzerRuleHandler> ruleHandlers,
            Dictionary<int, IAnalyzerOutputHandler> outputHandlers,
            Dictionary<int, IAnalyzerScheduleHandler> scheduleHandlers)
        {
            _analyzerConfig = analyzerConfig;

            _logsReceiver = logsReceiver;

            _ruleHandlers = ruleHandlers;

            _outputHandlers = outputHandlers;

            _scheduleHandlers = scheduleHandlers;

            _ruleHandlers.Values.ToList().ForEach(arh => arh.OnAnalyzerResult += OnAnalyzerResult);

            if (logsReceiver != null)
            {
                logsReceiver.OnNewLogMessage += LogsReceiver_OnNewLogMessage;
            }
        }

        public void Start()
        {
            _logsReceiver?.Start();

            Task consumerThread = Task.Factory.StartNew(() =>
            {
                while (!_analyzerLogsCollection.IsCompleted)
                {
                    if (_analyzerLogsCollection.TryTake(out LogMessage logMessage))
                    {
                        if (IsLogAnalyzingRequired())
                        {
                            _ruleHandlers.Values.ToList().ForEach(r =>
                            {
                                if (IsValidTime(r.RuleID))
                                {
                                    r.HandleLog(logMessage);
                                }
                            });
                        }
                    }
                }
            });
        }

        private bool IsValidTime(int ruleID)
        {
            return _scheduleHandlers.FirstOrDefault(ash => _analyzerConfig.RulesSchedules.ContainsKey(ruleID) &&
                                ash.Key == _analyzerConfig.RulesSchedules[ruleID]).Value?.IsValid ?? false;
        }

        private void LogsReceiver_OnNewLogMessage(object sender, LogMessage logMessage)
        {
            AnalyzeLog(logMessage);
        }

        public void AnalyzeLog(LogMessage logMessage)
        {
            _analyzerLogsCollection.TryAdd(logMessage);
        }

        private void OnAnalyzerResult(object sender, AnalyzerResultEventArgs e)
        {
            if (e != null)
            {
                var outputID = _analyzerConfig.RulesOutputs[e.RuleID];

                var outputHandler = _outputHandlers.FirstOrDefault(oh => oh.Key == outputID).Value;

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

        private bool IsLogAnalyzingRequired()
        {
            return _analyzerConfig != null && _analyzerConfig.IsEnabled && _analyzerConfig.Rules.Any(ar => ar.IsEnabled);
        }

        public void Dispose()
        {
            _analyzerLogsCollection?.CompleteAdding();

            _outputHandlers?.ToList().ForEach(aoh => aoh.Value?.Dispose());

            _ruleHandlers?.ToList().ForEach(rh => rh.Value?.Dispose());

            _outputHandlers?.Clear();

            _ruleHandlers?.Clear();

            _scheduleHandlers?.Clear();
        }
    }
}
