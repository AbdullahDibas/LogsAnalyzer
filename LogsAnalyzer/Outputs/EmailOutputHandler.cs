using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Outputs;
using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LogsManager.Analyzer.Outputs
{
    public class EmailOutputHandler : IAnalyzerOutputHandler
    {
        private EmailOutputConfig _emailOutputConfig;

        private SmtpClient _smtpClient;

        public int OutputID { get; set; }

        public EmailOutputHandler(EmailOutputConfig emailOutputConfig)
        {
            _emailOutputConfig = emailOutputConfig;
        }

        public void Output(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            MailMessage mailMessage = new MailMessage(_emailOutputConfig.FromEmail, _emailOutputConfig.ToEmail, "Subject", "Body");

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailOutputConfig.FromEmail, _emailOutputConfig.FromEmailPassword)
            };
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            { 
                Console.Write(ex.Message);
            }
        }
        public void Dispose()
        {
        }
    }
}
