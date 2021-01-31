using LogsManager.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer
{
    public class AnalyzerRuleConfig
    {
       public int ID { get; set; }

       public bool IsEnabled { get; set; }

       public LogAnalysisTypes AnalysisType { get; set; }
    }
}
