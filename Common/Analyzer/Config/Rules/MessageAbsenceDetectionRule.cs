using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Config.Rules
{
    public class MessageAbsenceDetectionRule : AnalyzerRuleConfig
    {
        public int LogMessageID { get; set; }
        public int AbsenceIntervalInSeconds { get; set; }
    }
}
