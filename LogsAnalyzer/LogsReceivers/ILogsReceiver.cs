using System;
using LogsManager.Common;

namespace LogsManager.Analyzer
{
    internal interface ILogsReceiver
    {
        void Start();

        void Stop();

        event EventHandler<LogMessage> OnNewLogMessage;
    }
}
