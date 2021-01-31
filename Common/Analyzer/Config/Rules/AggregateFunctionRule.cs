using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class AggregateFunctionRule : AnalyzerRuleConfig
    {
        public int LogMessageID { get; set; }

        public string ParamName { get; set; }

        public AggregateFunctions AggregateFunction { get; set; }

        public int[] ConditionsIDs { get; set; }
    }
}
