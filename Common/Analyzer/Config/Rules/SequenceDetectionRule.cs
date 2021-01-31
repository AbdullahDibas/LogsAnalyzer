using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class ISequenceDetectionRule : AnalyzerRuleConfig
    {
        public int[] SortedLogMessagesIDs { get; set; }
    }
}
