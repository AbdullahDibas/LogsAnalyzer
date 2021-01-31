using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class LogFilterRule : AnalyzerRuleConfig
    {
        public int[] LogMessagesIDs { get; set; }
    }
}
