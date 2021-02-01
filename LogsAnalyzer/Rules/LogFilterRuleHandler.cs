using System;
using System.Linq;
using LogsManager.Common;
using LogsManager.Common.Analyzer;

namespace LogsManager.Analyzer.Rules
{
    /// <summary>
    /// represents a log rule that outputs the received messages that pass the filters.
    /// </summary>
    internal class LogFilterRuleHandler : IAnalyzerRuleHandler
    {
        /// <summary>
        /// the configurations array that contain the filters applied on the received messages.
        /// </summary>
        private readonly LogMessageConfig[] _logMessages;

        public LogFilterRuleHandler(int id, Common.Analyzer.Rules.LogFilterRule logFilterRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            _logMessages =  logFilterRule?.LogMessagesIDs?.Select(logId => analyzerConfig.GetLogMessageConfig(logId)).ToArray();
        }

        public int RuleID { get; private set; }

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {
            bool doesMatchLogsMessages = _logMessages.Any(lm => LogMatchHelper.DoesMatch(lm, logMessage));

            if (doesMatchLogsMessages)
            {
                AnalyzerResultEventArgs analyzerResultEventArgs = new AnalyzerResultEventArgs
                {
                    Messages = new LogMessage[] { logMessage }
                };

                analyzerResultEventArgs.RuleID = RuleID;

                OnAnalyzerResult.Invoke(this, analyzerResultEventArgs);
            }
        }

        public void Dispose()
        { 
        }
    }
}
