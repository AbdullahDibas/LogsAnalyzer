using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer
{
    public class AnalyzerConfig
    {
        public bool IsEnabled { get; set; }

        public LogMessageConfig[] LogMessages { get; set; }

        public AnalyzerOutputConfig[] Outputs { get; set; }

        public AnalyzerScheduleConfig[] Schedules { get; set; }

        public  AnalyzerRuleConfig[] Rules { get; set; }

        public AnalyzerConditionConfig[] Conditions { get; set; }

        public Dictionary<int, int> RulesOutputs { get; set; }

        public Dictionary<int, int> RulesSchedules { get; set; }
    }
}
