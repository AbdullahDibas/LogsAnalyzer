using LogsManager.Common.Analyzer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LogsManager.Analyzer.Schedule
{
    public class AnalyzerScheduleHandler : IAnalyzerScheduleHandler
    {
        private DateTime _validDateTime;

        private AnalyzerScheduleConfig _scheduleConfig;

        public event EventHandler<bool> OnScheduleStatusChanged;

        public int ScheduleID { get; set; }

        public AnalyzerScheduleHandler(AnalyzerScheduleConfig scheduleConfig)
        {
            ScheduleID = scheduleConfig.ID;

            _scheduleConfig = scheduleConfig;

            if (_scheduleConfig.ScheduleType == Common.Enums.ScheduleTypes.OneTimeOnly)
            {
                _validDateTime = DateTime.Now;
            }
        }

        public bool IsValid 
        {
            get 
            {
                if (_scheduleConfig.ScheduleType == Common.Enums.ScheduleTypes.Daily)
                {
                    _validDateTime = DateTime.Now;
                }
                
                return _scheduleConfig.ScheduleType == Common.Enums.ScheduleTypes.Always
                    || _validDateTime.TimeOfDay > _scheduleConfig.StartTimeSpan && _validDateTime.TimeOfDay < _scheduleConfig.EndTimeSpan;
            } 
        }
    }
}
