using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Outputs
{
    public class EmailOutputConfig : AnalyzerOutputConfig
    {
        public string FromEmail { get; set; }
        public string FromEmailPassword { get; set; }
        public string ToEmail { get; set; }
        public string SmtpServerHost { get; set; }
        public int SmtpServerPort { get; set; }
    }
}
