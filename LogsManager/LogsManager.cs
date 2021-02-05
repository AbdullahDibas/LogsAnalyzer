using System;
using System.IO;
using System.Timers;
using System.IO.Pipes;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using LogsManager.Common;
using LogsManager.Common.Enums;

namespace LogsManager
{
    public class LogsManager : ILogsManager
    {
        #region Declarations
        private bool _isLogAnalyzerRunning = false;
        private const int TIMER_INTERVAL = 10000;
        private Timer _checkAnalyzerTimer;
        private Func<bool> _isLogAnalyzerEnabled;
        private readonly ILogsSender _logsSender;

        #endregion

        public LogsManager(Func<bool> isLogAnalyzerEnabled, ILogsSender logsSender)
        {
            _isLogAnalyzerEnabled = isLogAnalyzerEnabled;

            _logsSender = logsSender;
        }

        public void Initialize()
        { 
            LoadLogsAnalyzerSettings();

            _checkAnalyzerTimer = new Timer(TIMER_INTERVAL);

            _checkAnalyzerTimer.Elapsed += CheckAnalyzerTimer_Elapsed;

            _checkAnalyzerTimer.Start();
        }
        
        private void CheckAnalyzerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
           _checkAnalyzerTimer.Stop();

            bool isLogAnalyzerEnabled = LoadLogsAnalyzerSettings();

            if (isLogAnalyzerEnabled && !_logsSender.IsRunning())
            {
                _logsSender.Start();

                _isLogAnalyzerRunning = _logsSender.IsRunning();
            }

            _checkAnalyzerTimer.Start();
        }

        private bool LoadLogsAnalyzerSettings()
        {
            bool isLoaded = false;

            try
            { 
                isLoaded = (_isLogAnalyzerEnabled?.Invoke()?? false) && _logsSender.IsEnabled();
            }
            catch
            { }

            return isLoaded;
        } 

        public void Debug(string message, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            HandleLogMessage(LogLevels.Debug, message, null, tags, parameters);
        }

        public void Error(string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            HandleLogMessage(LogLevels.Error, message, exception, tags, parameters);
        }

        public void Fatal(string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            HandleLogMessage(LogLevels.Fatal, message, exception, tags, parameters);
        }

        public void Info(string message, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            HandleLogMessage(LogLevels.Info, message, null, tags, parameters);
        }

        public void Warn(string message, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            HandleLogMessage(LogLevels.Warn, message, null, tags, parameters);
        }

        private void HandleLogMessage(LogLevels logLevel, string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            if(_isLogAnalyzerRunning)
            {
                _logsSender.SendLog(FormatLogMessage(logLevel, message, exception, tags, parameters));
            }
        }

        private LogMessage FormatLogMessage(LogLevels logLevel, string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            return new LogMessage 
            {
                DateTime = DateTime.Now, 
                Message = message,
                Exception = exception,
                LogLevel = logLevel, 
                Tags = tags,
                Parameters = parameters 
            };
        }
    }
}
