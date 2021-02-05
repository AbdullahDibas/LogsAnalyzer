using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using LogsManager.Common.Enums;
using LogsManager.Common.Analyzer.Outputs;

namespace LogsManager.Analyzer.Outputs
{
    public class FileOutputHandler : OutputHandlerBase
    {
        private bool _isFileAdded = false;
        private StreamWriter _fileStreamWriter;
        private readonly FileOutputConfig _fileOutputConfig;
        private const string MSG_SEPARATOR_LINE = "---------------------------------------------------------------------------";
        private const string OUTPUT_SEPARATOR_LINE = "===========================================================================";

        public FileOutputHandler(FileOutputConfig fileOutputConfig)
        {
            _fileOutputConfig = fileOutputConfig;
        }

        protected override void ProcessOutput(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            if (!_isFileAdded)
            {
                _isFileAdded = true;

                _fileStreamWriter = new StreamWriter(_fileOutputConfig.FilePath, true)
                {
                    AutoFlush = true
                };
            }

            _fileStreamWriter?.WriteLine(GetFormattedLogMessage(messageParameters, analysisParameters));
        }

        public string GetFormattedLogMessage(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            string formattedMessage = "";

            var outputMessageParameters = messageParameters?.Where(mp => _fileOutputConfig.IncludedMessageParameters?.Any(imp => mp.Keys.Contains(imp))?? true).ToList();
           
            var outputAnalysisParameters = analysisParameters?.Where(ap => _fileOutputConfig.IncludedAnalysisParameters?.Contains(ap.Key)?? true).ToList();

            outputMessageParameters?.ForEach(messageParams =>
            {
                messageParams?.ToList().ForEach(param => formattedMessage += param.Key.ToString() + " : " + param.Value + Environment.NewLine);

                if (!string.IsNullOrEmpty(formattedMessage))
                {
                    formattedMessage += MSG_SEPARATOR_LINE + Environment.NewLine;
                }
            });

            outputAnalysisParameters?.ForEach(param => formattedMessage += param.Key.ToString() + " : " + param.Value + Environment.NewLine);

            if (!string.IsNullOrEmpty(formattedMessage))
            {
                formattedMessage += OUTPUT_SEPARATOR_LINE;
            }

            return formattedMessage;
        }

        public override void Dispose()
        {
            _fileStreamWriter?.Close();

            _fileStreamWriter?.Dispose();

            _fileStreamWriter = null;
        }
    }
}
