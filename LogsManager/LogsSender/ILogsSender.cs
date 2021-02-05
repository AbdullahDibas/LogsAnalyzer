using LogsManager.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager
{
    public interface ILogsSender
    {
        bool IsEnabled();

        bool IsRunning();

        void Start();

        void SendLog(LogMessage logMessage);
    }
}
