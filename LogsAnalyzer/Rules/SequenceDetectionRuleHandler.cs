using System;
using System.Linq;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Rules;

namespace LogsManager.Analyzer.Rules
{ 
    /// <summary>
    /// represents a log rule that searches for matches of a specific sequence of logs.
    /// </summary>
    public class SequenceDetectionRuleHandler : IAnalyzerRuleHandler
    {
        /// <summary>
        /// the index of the next expected log in the sequence logs array.
        /// </summary>
        private int _nextExpectedLogIndex;

        public int RuleID { get; private set; }

        /// <summary>
        /// the configurations array of the sequence messages.
        /// </summary>
        private readonly LogMessageConfig[] sortedLogsMessages;

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        public SequenceDetectionRuleHandler(int id, ISequenceDetectionRule sequenceDetectionRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            sortedLogsMessages = analyzerConfig.LogMessages.Where(lm => lm != null && sequenceDetectionRule
            .SortedLogMessagesIDs.Contains(lm.ID)).ToArray();

            _nextExpectedLogIndex = 0;
        }

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {
            bool match = false;

            if (LogMatchHelper.DoesMatch(sortedLogsMessages[_nextExpectedLogIndex], logMessage))
            {
                match = true;
            }
            else if (LogMatchHelper.DoesMatch(sortedLogsMessages[0], logMessage))
            {
                _nextExpectedLogIndex = 0;
                match = true;
            }

            if (match)
            {
                if (_nextExpectedLogIndex < sortedLogsMessages.Length - 1)
                {
                    _nextExpectedLogIndex++;
                }
                else
                {
                    _nextExpectedLogIndex = 0;

                    AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs
                    {
                        RuleID = RuleID
                    };

                    OnAnalyzerResult.Invoke(this, eventArgs);
                }
            }
        }
    }
}
