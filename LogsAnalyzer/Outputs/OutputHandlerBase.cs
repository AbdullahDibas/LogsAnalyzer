using LogsManager.Common.Analyzer;
using LogsManager.Common.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LogsManager.Analyzer.Outputs
{
    public abstract class OutputHandlerBase : IAnalyzerOutputHandler
    {
        ConcurrentQueue<Action> _outputProcesses = new ConcurrentQueue<Action>();

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

        public int OutputID { get; set; }

        public void Output(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            _outputProcesses.Enqueue(() => ProcessOutput(messageParameters, analysisParameters));
        }

        protected abstract void ProcessOutput(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters);

        public abstract void Dispose();
    }
}
