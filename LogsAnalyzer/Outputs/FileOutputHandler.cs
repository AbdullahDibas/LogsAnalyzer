using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Enums;
using LogsManager.Common.Analyzer.Outputs;

namespace LogsManager.Analyzer.Outputs
{
    public class FileOutputHandler : IAnalyzerOutputHandler
    {
        private readonly FileOutputConfig _fileOutputConfig;
        private StreamWriter _fileStreamWriter;
        private readonly object _synchLock;

        public int OutputID { get; set; }

        public FileOutputHandler(FileOutputConfig fileOutputConfig)
        {
            _fileOutputConfig = fileOutputConfig;

            _synchLock = new object();

            _fileStreamWriter = new StreamWriter(_fileOutputConfig.FilePath, true)
            {
                AutoFlush = true
            };
        }

        public void Output(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            lock (_synchLock)
            {
                _fileStreamWriter?.WriteLine(GetFormattedLogMessage(messageParameters, analysisParameters));
            }
        }

        public string GetFormattedLogMessage(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            string formattedMessage = "";

            if (messageParameters?.Count() > 0)
            {
                for (int i = 0; i < messageParameters.Count(); i++)
                {
                    foreach (var parameter in messageParameters[i])
                    {
                        if (_fileOutputConfig.IncludedMessageParameters.ToList().Contains(parameter.Key))
                        {
                            formattedMessage += parameter.Key.ToString() + " : " + parameter.Value;
                            formattedMessage += Environment.NewLine;
                        }
                    }

                    if (!string.IsNullOrEmpty(formattedMessage))
                    {
                        formattedMessage += "---------------------------------------------------------------------------";
                        formattedMessage += Environment.NewLine;
                    }
                }
            }

            if (analysisParameters?.Count > 0)
            {
                foreach (var parameter in analysisParameters)
                {
                    if (_fileOutputConfig.IncludedAnalysisParameters?.ToList().Contains(parameter.Key)??true)
                    {
                        formattedMessage += parameter.Key.ToString() + " : " + parameter.Value;
                        formattedMessage += Environment.NewLine;
                    }
                }
            }

            if (!string.IsNullOrEmpty(formattedMessage))
            {
                formattedMessage += "===========================================================================";
            }

            return formattedMessage;
        }

        public void Dispose()
        {
            lock (_synchLock)
            {
                _fileStreamWriter?.Close();

                _fileStreamWriter?.Dispose();

                _fileStreamWriter = null;
            }
        }
    }
}
