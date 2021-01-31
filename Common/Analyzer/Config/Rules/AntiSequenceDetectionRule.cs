using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class IAntiSequenceDetectionRule : AnalyzerRuleConfig
    {
        public int[] SortedLogMessagesIDs { get; set; }
    }
}
