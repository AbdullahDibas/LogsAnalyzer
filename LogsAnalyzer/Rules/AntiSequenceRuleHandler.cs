using System;
using System.Linq;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Rules;

namespace LogsManager.Analyzer.Rules
{
    public class AntiSequenceRuleHandler : IAnalyzerRuleHandler
    {
        public int RuleID { get; private set; }

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        private LogMessageConfig[] sortedLogsMessages;

        private int _nextExpectedLogIndex;

        public AntiSequenceRuleHandler(int id, IAntiSequenceDetectionRule antiSequenceDetectionRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            sortedLogsMessages = analyzerConfig.LogMessages.Where(lm => lm != null && antiSequenceDetectionRule
            .SortedLogMessagesIDs.Contains(lm.ID)).ToArray();

            _nextExpectedLogIndex = 0;
        }

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {
            if (LogMatchHelper.DoesMatch(sortedLogsMessages[_nextExpectedLogIndex], logMessage))
            {
                if (_nextExpectedLogIndex < sortedLogsMessages.Length - 1)
                {
                    _nextExpectedLogIndex++;
                }
                else
                {
                    _nextExpectedLogIndex = 0;
                }
            }
            else if (_nextExpectedLogIndex > 0 && DoesMatchPreviousLog(logMessage))
            {
                AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs 
                {
                    RuleID = RuleID
                };

                OnAnalyzerResult?.Invoke(this, eventArgs);
            }
        }


        private bool DoesMatchPreviousLog(LogMessage logMessage)
        {
            return sortedLogsMessages.Take(_nextExpectedLogIndex).Any(lm =>
            LogMatchHelper.DoesMatch(lm, logMessage));
        }

        public void Dispose()
        {
            sortedLogsMessages = null;
        }
    }
}
