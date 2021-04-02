using LogsManager.Analyzer;
using LogsManager.Analyzer.Factories;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager
{
    /// <summary>
    /// a class used to send logs to the analyzer when it's hosted in the same domin of the original application.
    /// </summary>
    public class DefaultSender : ILogsSender
    {
        ILogsAnalyzer _logsAnalyzer;
        bool _isRunning = false;

        public bool IsEnabled()
        {
            return true;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

        public void SendLog(LogMessage logMessage)
        {
            _logsAnalyzer.AnalyzeLog(logMessage);
        }

        public void Start()
        {
            AnalyzerConfig analyzerConfig = ConfigurationsFactory.GetAnalyzerConfig();

            if (analyzerConfig?.IsEnabled ?? false)
            {
                _isRunning = true;

                Dictionary<int, IAnalyzerRuleHandler> analyzerRuleHandlers = RuleHandlerFactory.CreateRulesHandlers(analyzerConfig);

                Dictionary<int, IAnalyzerOutputHandler> analyzerOutputHandlers = OutputsHandlersFactory.GetOutputsHandlers(analyzerConfig);

                Dictionary<int, IAnalyzerScheduleHandler> analyzerScheduleHandlers = SchedulesHandlersFactory.CreateSchedulesHandlers(analyzerConfig);

                _logsAnalyzer = new LogsAnalyzer(analyzerConfig, null, analyzerRuleHandlers, analyzerOutputHandlers, analyzerScheduleHandlers);

                _logsAnalyzer.Start();
            }
        }
    }
}
