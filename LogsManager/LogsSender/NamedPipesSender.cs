﻿using LogsManager.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace LogsManager
{
    /// <summary>
    /// a class is used to send the logs using named pipes to the analyzer when the analyzer is hosted in a separate application.
    /// </summary>
    public class NamedPipesSender : ILogsSender
    {
        private TextWriter _pipeServerStreamWriter;

        private NamedPipeServerStream _logsManagerPipeServer;

        public bool IsEnabled()
        {
            return true;
        }

        public bool IsRunning()
        {
            return _logsManagerPipeServer?.IsConnected ?? false;
        }

        public void Start()
        {
            _logsManagerPipeServer = new NamedPipeServerStream("LogsAnalyzerPipe");

            _logsManagerPipeServer.WaitForConnection();

            _pipeServerStreamWriter = TextWriter.Synchronized(new StreamWriter(_logsManagerPipeServer) { AutoFlush = true });
        }

        public void SendLog(LogMessage logMessage)
        {
            if (_logsManagerPipeServer.IsConnected)
            {
                _pipeServerStreamWriter.WriteLine(JsonConvert.SerializeObject(logMessage));
            }
        }
    }
}
