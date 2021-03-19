using System;
using System.Linq;
using System.Diagnostics;
using LogsManager.Common;
using LogsManager.Analyzer;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Receivers;

namespace LogsManager.MircosoftEventViewerReceiver
{
    public class EventLogsReceiver : ILogsReceiver
    {
        WindowsEventsViewerReceiverConfig _logsReceiverConfig;

        private DateTime _startDateTime = DateTime.Now;

        private int _lastEventLogID;

        private bool _isStopRequested;

        private readonly System.Timers.Timer _logsReadTimer;

        public event EventHandler<LogMessage> OnNewLogMessage;

        public EventLogsReceiver(WindowsEventsViewerReceiverConfig logsReceiverConfig)
        {
            _logsReceiverConfig = logsReceiverConfig;

            _logsReadTimer = new System.Timers.Timer(100);

            _logsReadTimer.Elapsed += LogsReadTimer_Elapsed;
        }

        private void LogsReadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                _logsReadTimer.Stop();

                EventLog[] remoteEventLogs = _logsReceiverConfig.MachineHost == "" ? EventLog.GetEventLogs() : EventLog.GetEventLogs(_logsReceiverConfig.MachineHost);

                EventLog eventLog = remoteEventLogs.Where(ev => ev.LogDisplayName == _logsReceiverConfig.LogsCategoryName).First();

                foreach (EventLogEntry log in eventLog.Entries)
                {
                    if (log.Index > _lastEventLogID && log.TimeGenerated >= _startDateTime)
                    {
                        _lastEventLogID = log.Index;

                        if (!_isStopRequested)
                        {
                            HandleNewLog(log);
                        }
                    }
                }
            }
            finally
            {
                _logsReadTimer.Start();
            }
        }

        private void HandleNewLog(EventLogEntry log)
        {
            OnNewLogMessage?.Invoke(null, new LogMessage
            {
                DateTime = log.TimeWritten,
                Message = log.Message,
                LogLevel = log.EntryType == EventLogEntryType.Error ? LogsManager.Common.Enums.LogLevels.Error : 
                   (log.EntryType == EventLogEntryType.Information ? LogsManager.Common.Enums.LogLevels.Info : 
                   (log.EntryType == EventLogEntryType.Warning? LogsManager.Common.Enums.LogLevels.Warn: LogsManager.Common.Enums.LogLevels.Info)),
                Tags = new string[] { log.Category }

            }) ;
        }

        public void Start()
        {
            _logsReadTimer.Start();
        }

        public void Stop()
        {
            _logsReadTimer.Stop();
        }
    }
}
