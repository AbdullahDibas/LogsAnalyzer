using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Outputs
{
    public class EmailOutputConfig : AnalyzerOutputConfig
    {
        public string SmtpServer { get; set; }
        public string FromEmail { get; set; }
        public string FromEmailPassword { get; set; }
        public string ToEmail { get; set; }
    }
}
