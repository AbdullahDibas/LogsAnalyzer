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
        private string _analyzerAssemblyFileName = "LogsAnalyzer\\LogsManager.Analyzer"; 
        private const int TIMER_INTERVAL = 10000;
        private Timer _checkAnalyzerTimer;
        private Process _logsAnalyzerProcess;
        private readonly TextWriter _pipeServerStreamWriter;
        private readonly AnonymousPipeServerStream _logsManagerPipeServer;
        private Func<bool> _isLogAnalyzerEnabled;
        #endregion

        public LogsManager(Func<bool> isLogAnalyzerEnabled)
        {
            _isLogAnalyzerEnabled = isLogAnalyzerEnabled;

            _logsManagerPipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

            _pipeServerStreamWriter = TextWriter.Synchronized(new StreamWriter(_logsManagerPipeServer) { AutoFlush  = true });
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

            if (isLogAnalyzerEnabled && !IsLogAnalyzerRunning())
            {
                StartLogsAnalyzerProcess();

                _isLogAnalyzerRunning = IsLogAnalyzerRunning();
            }

            _checkAnalyzerTimer.Start();
        }

        private bool LoadLogsAnalyzerSettings()
        {
            bool isLoaded = false;

            try
            { 
                isLoaded = (_isLogAnalyzerEnabled?.Invoke()?? false) && File.Exists(_analyzerAssemblyFileName + ".exe");
            }
            catch
            { }

            return isLoaded;
        }

        private bool IsLogAnalyzerRunning()
        {
            Process[] pname = Process.GetProcessesByName("LogsManager.Analyzer");

            return pname.Length > 0;
        }

        private void StartLogsAnalyzerProcess()
        {
            _logsAnalyzerProcess = new Process();

            _logsAnalyzerProcess.StartInfo.FileName = _analyzerAssemblyFileName + ".exe";

            // Pass the client process a handle to the server.
            _logsAnalyzerProcess.StartInfo.Arguments = _logsManagerPipeServer.GetClientHandleAsString();

            _logsAnalyzerProcess.StartInfo.UseShellExecute = false;

            _logsAnalyzerProcess.Start();

            _logsManagerPipeServer.DisposeLocalCopyOfClientHandle();
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
                SendLogToAnalyzer(FormatLogMessage(logLevel, message, exception, tags, parameters));
            }
        }

        private string FormatLogMessage(LogLevels logLevel, string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            LogMessage logMessage = new LogMessage 
            {
                DateTime = DateTime.Now, 
                Message = message,
                Exception = exception,
                LogLevel = logLevel, 
                Tags = tags,
                Parameters = parameters 
            };

            return JsonConvert.SerializeObject(logMessage);
        }

        private void SendLogToAnalyzer(string formattedLogMessage)
        {
            try
            {
                _pipeServerStreamWriter.WriteLine(formattedLogMessage);
                    
                _logsManagerPipeServer.WaitForPipeDrain();  
            }
            catch (IOException e)
            {
                Console.WriteLine("[SERVER] Error: {0}", e.Message);
            }
        }
    }
}
