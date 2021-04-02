using System;
using System.Collections.Generic;
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

        private List<LogMessage> _sequenceMessages;

        public int RuleID { get; private set; }

        /// <summary>
        /// the configurations array of the sequence messages.
        /// </summary>
        private readonly LogMessageConfig[] sortedLogsMessages;

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        public SequenceDetectionRuleHandler(int id, ISequenceDetectionRule sequenceDetectionRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            sortedLogsMessages = sequenceDetectionRule?.SortedLogMessagesIDs?
                .Select(logId => analyzerConfig.GetLogMessageConfig(logId)).ToArray();
            
            _nextExpectedLogIndex = 0;
        }

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {
            bool match = false;

            // check if the log message matches the next message configuration in the sequence
            if (LogMatchHelper.DoesMatch(sortedLogsMessages[_nextExpectedLogIndex], logMessage))
            {
                match = true;
            }
            // check if the message matches the first one, if yes check the sequence again from the start
            else if (LogMatchHelper.DoesMatch(sortedLogsMessages[0], logMessage))
            {
                _nextExpectedLogIndex = 0;
                match = true;
            }

            if (match)
            {
                _sequenceMessages.Add(logMessage);

                if (_nextExpectedLogIndex < sortedLogsMessages.Length - 1)
                {
                    _nextExpectedLogIndex++;
                }
                else // if the last message in the sequence is received.
                {
                    _nextExpectedLogIndex = 0;

                    AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs
                    {
                        RuleID = RuleID,
                        Messages = _sequenceMessages.Select(lm => lm.Copy()).ToArray()
                    };

                    OnAnalyzerResult.Invoke(this, eventArgs);

                    _sequenceMessages.Clear();
                }
            }
        }

        public void Dispose()
        {
            _sequenceMessages?.Clear();
        }
    }
}
