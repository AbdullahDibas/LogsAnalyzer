using System;

namespace LogsManager.Common.Analyzer
{
    public interface IAnalyzerScheduleHandler
    {
        int ScheduleID { get; set; }

        bool IsValid { get; }

        event EventHandler<bool> OnScheduleStatusChanged;
    }
}