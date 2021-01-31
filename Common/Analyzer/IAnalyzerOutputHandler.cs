using LogsManager.Common.Enums;
using System.Collections.Generic;

namespace LogsManager.Common.Analyzer
{
    public interface IAnalyzerOutputHandler
    {
        int OutputID { get; }
        void Output(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters);

        void Dispose();
    }
}
