using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Config.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogsManager.Analyzer.Rules
{
    public class MessageAbsenceDetectionRuleHandler : IAnalyzerRuleHandler
    {
        public int RuleID { get; private set; }

        private LogMessageConfig _logMessageConfig;

        private readonly System.Timers.Timer _monitoringTimer;

        private MessageAbsenceDetectionRule _messageAbsenceDetectionRule;

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        public MessageAbsenceDetectionRuleHandler(int id, MessageAbsenceDetectionRule messageAbsenceDetectionRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            if (messageAbsenceDetectionRule.AbsenceIntervalInSeconds > 0)
            {
                _monitoringTimer = new System.Timers.Timer(messageAbsenceDetectionRule.AbsenceIntervalInSeconds * 1000);

                _monitoringTimer.Elapsed += MonitoringTimer_Elapsed;

                _monitoringTimer.Start();

                _messageAbsenceDetectionRule = messageAbsenceDetectionRule;

                _logMessageConfig = analyzerConfig.LogMessages.FirstOrDefault(lm => lm != null && messageAbsenceDetectionRule.LogMessageID == lm.ID);
            }
        }

        private void MonitoringTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _monitoringTimer.Stop();

                AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs
                {
                    RuleID = RuleID,
                    AnalysisParameters = new Dictionary<string, string>
                    {
                        {"Date Time: ", DateTime.Now.ToString() },
                        {"Description", $"{_messageAbsenceDetectionRule} seconds have passed and the message hasn't been received yet."}
                    }
                };

                OnAnalyzerResult.Invoke(this, eventArgs);
            }
            finally
            {
                _monitoringTimer.Start();
            }
        }

        public void HandleLog(LogMessage logMessage)
        {
            if (LogMatchHelper.DoesMatch(_logMessageConfig, logMessage))
            {
                _monitoringTimer.Stop();

                _monitoringTimer.Start();
            }
        }

        public void Dispose()
        {
            _monitoringTimer.Elapsed -= MonitoringTimer_Elapsed;

            _monitoringTimer.Stop();
        }
    }
}
