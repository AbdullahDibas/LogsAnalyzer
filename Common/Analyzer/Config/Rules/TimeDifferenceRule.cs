using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class TimeDifferenceRule : AnalyzerRuleConfig
    {
        public int FirstLogMessageID { get; set; }
        public int SecondLogMessageID { get; set; }
        public int[] ConditionsIDs { get; set; }
    }
}