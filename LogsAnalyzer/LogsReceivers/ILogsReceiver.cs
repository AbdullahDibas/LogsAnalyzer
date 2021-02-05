using System;
using LogsManager.Common;

namespace LogsManager.Analyzer
{
    public interface ILogsReceiver
    {
        void Start();

        void Stop();

        event EventHandler<LogMessage> OnNewLogMessage;
    }
}
