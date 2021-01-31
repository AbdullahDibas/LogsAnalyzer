using System;
using System.Linq;
using System.Collections.Generic;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Rules;

namespace LogsManager.Analyzer.Rules
{
    /// <summary>
    /// represents a log rule that detects .
    /// </summary>
    public class DuplicateDetectionRuleHandler : IAnalyzerRuleHandler
    {
        /// <summary>
        /// the configurations that represent the messages to be processed.
        /// </summary>
        private readonly LogMessageConfig _logMessage;

        /// <summary>
        /// the configuration of the duplicate detection rule.
        /// </summary>
        private readonly DuplicateDetectionRule _duplicateDetectionRule;

        /// <summary>
        /// a list of the recevied messages that meet the configurations.
        /// </summary>
        private List<LogMessage> _monitoredLogMessages;

        public int RuleID { get; private set; }

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        public DuplicateDetectionRuleHandler(int id, DuplicateDetectionRule duplicateDetectionRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            _duplicateDetectionRule = duplicateDetectionRule;

            _logMessage = analyzerConfig.LogMessages.FirstOrDefault(lm => lm != null && duplicateDetectionRule.LogMessageID == lm.ID);

            _monitoredLogMessages = new List<LogMessage>();
        }

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {
            if (LogMatchHelper.DoesMatch(_logMessage, logMessage))
            {
                var originalMessage = _monitoredLogMessages.FirstOrDefault(lm => LogMatchHelper.IsTheSameMessage(lm, logMessage));
               
                if (originalMessage != null)
                {
                    AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs
                    {
                        RuleID = RuleID,
                        AnalysisParameters = new Dictionary<string, string>
                        {
                            {
                                "Original Date Time:", originalMessage.DateTime.ToString()
                            }
                        },
                        Messages = new LogMessage[] { logMessage }
                    };

                    OnAnalyzerResult.Invoke(this, eventArgs);
                }
                else
                {
                    if (_monitoredLogMessages.Count < _duplicateDetectionRule.MaximumNumberOfMonitoredMessages)
                    {
                        _monitoredLogMessages.Add(logMessage);
                    }
                }
            }
        }
    }
}
