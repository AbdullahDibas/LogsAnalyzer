using LogsManager.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Config
{
    public class LogMessageParameter
    {
        public LogMessageParameters FilterParameter { get; set; }

        public string FilterValue { get; set; }

        public TextCompareTypes TextCompareType { get; set; }
    }
}
