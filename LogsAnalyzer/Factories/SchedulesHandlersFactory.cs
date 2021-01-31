using LogsManager.Common.Analyzer;
using System.Collections.Generic;
using System.Linq;
using LogsManager.Analyzer.Schedule;

namespace LogsManager.Analyzer
{
    internal static class SchedulesHandlersFactory
    {
        public static Dictionary<int, IAnalyzerScheduleHandler> CreateSchedulesHandlers(AnalyzerConfig analyzerConfig)
        {
            Dictionary<int, IAnalyzerScheduleHandler> keyValuePairs = new Dictionary<int, IAnalyzerScheduleHandler>();

            analyzerConfig.Schedules.ToList().ForEach(s =>
            {
                IAnalyzerScheduleHandler analyzerScheduleHandler = new AnalyzerScheduleHandler(s);
                
                keyValuePairs.Add(s.ID, analyzerScheduleHandler);
            });

            return keyValuePairs;
        }
    }
}