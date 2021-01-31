using LogsManager.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer
{
    public class AnalyzerScheduleConfig
    {
        public int ID { get; set; }

        public ScheduleTypes ScheduleType { get; set; }

        public TimeSpan StartTimeSpan { get; set; }

        public TimeSpan EndTimeSpan { get; set; }
    }
}
