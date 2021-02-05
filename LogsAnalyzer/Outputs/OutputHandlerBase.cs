using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Enums;

namespace LogsManager.Analyzer.Outputs
{
    public abstract class OutputHandlerBase : IAnalyzerOutputHandler
    {
        public int OutputID { get; set; }

        private ConcurrentQueue<Action> _outputProcesses = new ConcurrentQueue<Action>();

        public OutputHandlerBase()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (_outputProcesses.Count > 0 && _outputProcesses.TryDequeue(out Action outputProcess))
                    { 
                        outputProcess.Invoke();
                    }
                }
            });
        }

        public void Output(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            _outputProcesses.Enqueue(() => ProcessOutput(messageParameters, analysisParameters));
        }

        protected abstract void ProcessOutput(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters);

        public abstract void Dispose();
    }
}
