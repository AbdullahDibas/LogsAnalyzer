using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Outputs;
using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;

namespace LogsManager.Analyzer.Outputs
{
    public class HTMLFileOutputHandler : OutputHandlerBase
    {
        private bool _isFileAdded = false;
        private StreamWriter _fileStreamWriter;
        private bool _areHeadersWritten = false;
        private readonly object _synchLock;
        private readonly FileOutputConfig _fileOutputConfig;
        private readonly string _resourceName = "LogsManager.Analyzer.Outputs.Template2.html";
        private readonly string panelTemplate = "<script> addLog(\"{0}\", \"{1}\", \"{2}\");</script>";
        
        public HTMLFileOutputHandler(FileOutputConfig fileOutputConfig)
        {
            _fileOutputConfig = fileOutputConfig;

            _synchLock = new object();
        }

        protected override void ProcessOutput(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            lock (_synchLock)
            {
                if (!_isFileAdded)
                {
                    _isFileAdded = true;

                    _fileStreamWriter = new StreamWriter(_fileOutputConfig.FilePath, true)
                    {
                        AutoFlush = true
                    };
                }

                for (int i = 0; i < messageParameters.Length; i++)
                {
                    if (!_areHeadersWritten)
                    {
                        _areHeadersWritten = true;

                        var assembly = Assembly.GetExecutingAssembly();

                        using (Stream stream = assembly.GetManifestResourceStream(_resourceName))

                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string result = reader.ReadToEnd();

                            _fileStreamWriter.WriteLine(result);
                        }
                    }

                    var logLevel = GetMessageLogLevel(messageParameters[i]);

                    _fileStreamWriter.WriteLine(string.Format(panelTemplate, logLevel.ToString() + " - " + GetLogDateTime(messageParameters[i]),
                        GetLogMessage(messageParameters[i]), logLevel == LogLevels.Info ? "info" : (logLevel == LogLevels.Error ? "danger" : "warning")));
                }
            }
        }

        private LogLevels GetMessageLogLevel(Dictionary<LogMessageParameters, string> messageParameters)
        {
            return  (LogLevels)Enum.Parse(typeof(LogLevels), messageParameters.FirstOrDefault(x => x.Key == LogMessageParameters.Level).Value);
        }

        private string GetLogMessage(Dictionary<LogMessageParameters, string> messageParameters)
        {
            return messageParameters.FirstOrDefault(x => x.Key == LogMessageParameters.Message).Value;
        }

        private string GetLogDateTime(Dictionary<LogMessageParameters, string> messageParameters)
        {
            return messageParameters.FirstOrDefault(x => x.Key == LogMessageParameters.DateTime).Value;
        }

        public override void Dispose()
        {
            _fileStreamWriter.Close();
            _fileStreamWriter.Dispose();
        }
    }
}
