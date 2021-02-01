using System;
using System.Linq;
using System.Collections.Generic;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Rules; 

namespace LogsManager.Analyzer.Rules
{
    /// <summary>
    /// represents a log rule that calculates the time difference between two logs.
    /// </summary>
    public class TimeDifferenceRuleHandler : IAnalyzerRuleHandler
    {
        /// <summary>
        /// the configurations that represent the first message.
        /// </summary>
        private LogMessageConfig _firstMessageConfig;

        /// <summary>
        /// the configurations that represent the second message.
        /// </summary>
        private LogMessageConfig _secondMessageConfig;

        /// <summary>
        /// is the first message received.
        /// </summary>
        private bool _isFirstMessageReceived = false;

        /// <summary>
        /// the datetime at which the last message that matches the first message configurations received.
        /// </summary>
        private DateTime _firstMessageDateTime;

        /// <summary>
        /// the received message that matches the first message configurations.
        /// </summary>
        private LogMessage _firstMessage;

        public int RuleID { get; private set; }

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        public TimeDifferenceRuleHandler(int id, TimeDifferenceRule timeDifferenceRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            _firstMessageConfig =  analyzerConfig.GetLogMessageConfig(timeDifferenceRule.FirstLogMessageID);

            _secondMessageConfig =  analyzerConfig.GetLogMessageConfig(timeDifferenceRule.SecondLogMessageID);
        }

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {            
            if (!_isFirstMessageReceived)
            {
                if (LogMatchHelper.DoesMatch(_firstMessageConfig, logMessage))
                {
                    _firstMessageDateTime = DateTime.Now;

                    _isFirstMessageReceived = true;

                    _firstMessage = logMessage;
                }                
            }
            else
            {
                bool doesMathSecondMessage = LogMatchHelper.DoesMatch(_secondMessageConfig, logMessage);

                if (doesMathSecondMessage)
                {
                    AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs
                    {
                        RuleID = RuleID,
                        AnalysisParameters = new Dictionary<string, string>
                        {
                            {
                                "Time Difference", DateTime.Now.Subtract(_firstMessageDateTime).TotalMilliseconds.ToString()
                            }
                        },
                        Messages = new LogMessage[]  {  _firstMessage, logMessage  }
                    };

                    OnAnalyzerResult.Invoke(this, eventArgs);

                    _isFirstMessageReceived = false;
                }
            }
        }

        public void Dispose()
        {
            _firstMessageConfig = null;

            _secondMessageConfig = null;
        }
    }
}
