using LogsManager.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace LogsManager
{
    /// <summary>
    /// a class is used to send the logs using anonymoius pipes to the analyzer when the analyzer is hosted in a separate application.
    /// </summary>
    public class AnonymousPipesSender : ILogsSender
    {
        private Process _logsAnalyzerProcess;
        private TextWriter _pipeServerStreamWriter;
        private AnonymousPipeServerStream _logsManagerPipeServer;
        private string _analyzerAssemblyFileName = "LogsAnalyzer\\LogsManager.Analyzer";

        public bool IsEnabled()
        {
            return File.Exists(_analyzerAssemblyFileName + ".exe");
        }

        public bool IsRunning()
        {
            Process[] pname = Process.GetProcessesByName("LogsManager.Analyzer");

            return pname.Length > 0;
        }

        public void Start()
        {
            _logsManagerPipeServer = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);

            _pipeServerStreamWriter = TextWriter.Synchronized(new StreamWriter(_logsManagerPipeServer) { AutoFlush = true });

            _logsAnalyzerProcess = new Process();

            _logsAnalyzerProcess.StartInfo.FileName = _analyzerAssemblyFileName + ".exe";

            // Pass the client process a handle to the server.
            _logsAnalyzerProcess.StartInfo.Arguments = _logsManagerPipeServer.GetClientHandleAsString();

            _logsAnalyzerProcess.StartInfo.UseShellExecute = false;

            _logsAnalyzerProcess.Start();

            _logsManagerPipeServer.DisposeLocalCopyOfClientHandle();
        }

        public void SendLog(LogMessage logMessage)
        {
            try
            {
                _pipeServerStreamWriter.WriteLine(JsonConvert.SerializeObject(logMessage));

                _logsManagerPipeServer.WaitForPipeDrain();
            }
            catch (IOException e)
            {
                Console.WriteLine("[SERVER] Error: {0}", e.Message);
            }
        }
    }
}
