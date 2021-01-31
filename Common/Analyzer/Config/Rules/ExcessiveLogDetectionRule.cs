using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class ExcessiveLogDetectionRule : AnalyzerRuleConfig
    {
        int[] LogMessagesIDs { get; set; }
    }
}
