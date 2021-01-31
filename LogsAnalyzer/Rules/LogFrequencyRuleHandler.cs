using System;
using System.Linq;
using System.Timers;
using System.Collections.Generic;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Rules;

namespace LogsManager.Analyzer.Rules
{
    /// <summary>
    /// represents a log rule that calculates the frequency of a specific message.
    /// </summary>
    public class LogFrequencyRuleHandler : IAnalyzerRuleHandler
    {
        /// <summary>
        /// the configurations that represent the messages to be processed.
        /// </summary>
        private LogMessageConfig _logMessage;

        /// <summary>
        /// the configuration of the frequency rule.
        /// </summary>
        private readonly LogFrequencyRule _logFrequencyRule;

        /// <summary>
        /// the number of messages received that match the configurations within the last frequency interval.
        /// </summary>
        private int _messageCountWithinLastInterval = 0;

        /// <summary>
        /// the total number of messages received that match the configurations.
        /// </summary>
        private int _totalMessagesCount = 0;

        /// <summary>
        /// the count of the frequency interval so far.
        /// </summary>
        private int _intervalsCount = 0;

        /// <summary>
        /// the timer used to report the output of the rule.
        /// </summary>
        private Timer _frequencyCheckTimer = new Timer();

        public int RuleID { get; private set; }

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        public LogFrequencyRuleHandler(int id, LogFrequencyRule logFrequencyRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            _logFrequencyRule = logFrequencyRule;

            _logMessage = analyzerConfig.LogMessages.FirstOrDefault(lm => lm.ID == _logFrequencyRule.LogMessageID);

            _frequencyCheckTimer = new Timer(_logFrequencyRule.TimeInSeconds * 1000);

            _frequencyCheckTimer.Elapsed += FrequencyCheckTimer_Elapsed;

            _frequencyCheckTimer.Start();
        }

        private void FrequencyCheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _intervalsCount++;

            AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs
            { 
                RuleID = RuleID,
                AnalysisParameters = new Dictionary<string, string>
                {
                    { "Interval End Time: ", DateTime.Now.ToString() },
                    { "Message Interval Count: ", _messageCountWithinLastInterval.ToString() },
                    { "Message Intervals Average Count: ", (_totalMessagesCount/_intervalsCount).ToString() },
                }
            };

            OnAnalyzerResult?.Invoke(this, eventArgs);

            _messageCountWithinLastInterval = 0;
        }

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {
            if (LogMatchHelper.DoesMatch(_logMessage, logMessage))
            {
                _messageCountWithinLastInterval++;
                _totalMessagesCount++;
            }
        }
    }
}
