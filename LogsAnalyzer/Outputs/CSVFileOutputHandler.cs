using System;
using System.IO;
using System.Linq;
using System.Text;
using LogsManager.Common.Enums;
using System.Collections.Generic;
using LogsManager.Common.Analyzer.Outputs;

namespace LogsManager.Analyzer.Outputs
{
    public class CSVFileOutputHandler : OutputHandlerBase
    {
        private bool _isFileAdded = false;
        private const string DELIMITER = ",";
        private bool _areHeadersWritten = false;
        private StreamWriter _fileStreamWriter;
        private readonly object _synchLock;
        private readonly FileOutputConfig _fileOutputConfig;

        public CSVFileOutputHandler(FileOutputConfig fileOutputConfig)
        {
            _fileOutputConfig = fileOutputConfig;

            _synchLock = new object();
        }

        protected override void ProcessOutput(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            lock (_synchLock)
            {
                if (!_areHeadersWritten)
                {
                    _fileStreamWriter?.WriteLine(string.Join(DELIMITER, GetHeaders(analysisParameters.Keys.ToList())));

                    _areHeadersWritten = true;
                }

                _fileStreamWriter?.WriteLine(GetFormattedLogMessage(messageParameters, analysisParameters));
            }
        }

        public List<string> GetHeaders(List<string> analysisHeaders)
        {
            List<string> headers = new List<string>();

            if (_fileOutputConfig.IncludedMessageParameters?.Length > 0)
            {
                headers.AddRange(_fileOutputConfig.IncludedMessageParameters.Select(p => p.ToString()));
            }

            if (_fileOutputConfig.IncludedAnalysisParameters?.Length > 0)
            {
                headers.AddRange(_fileOutputConfig.IncludedAnalysisParameters.Select(p => p.ToString()));
            }
            else 
            {
                headers.AddRange(analysisHeaders);
            }

            return headers;
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
                            formattedMessage += "\"" + parameter.Value + "\"";
                            formattedMessage += DELIMITER;
                        }
                    }
                }
            }

            if (analysisParameters?.Count > 0)
            {
                foreach (var parameter in analysisParameters)
                {
                    if (_fileOutputConfig.IncludedAnalysisParameters?.ToList().Contains(parameter.Key)??true)
                    {
                        formattedMessage += "\"" + parameter.Value + "\"";
                        formattedMessage += DELIMITER;
                    }
                }
            }

            return formattedMessage;
        }

        public override void Dispose()
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
