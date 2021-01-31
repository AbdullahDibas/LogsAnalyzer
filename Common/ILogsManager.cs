using System;
using System.Collections.Generic;

namespace LogsManager.Common
{
    /// <summary>
    /// an interface responsible on logging and\or analyzing traces.
    /// </summary>
    public interface ILogsManager
    {
        void Initialize();
        void Debug(string message, string[] tags = null, params KeyValuePair<string, string>[] parameters);
        void Info(string message, string[] tags = null, params KeyValuePair<string, string>[] parameters);
        void Warn(string message, string[] tags = null, params KeyValuePair<string, string>[] parameters);
        void Error(string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters);
        void Fatal(string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters);
    }
}
