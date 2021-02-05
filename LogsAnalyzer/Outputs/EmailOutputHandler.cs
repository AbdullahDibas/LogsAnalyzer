using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Outputs;
using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LogsManager.Analyzer.Outputs
{
    public class EmailOutputHandler : OutputHandlerBase
    {
        private readonly SmtpClient _smtpClient;

        private readonly EmailOutputConfig _emailOutputConfig;
         
        public EmailOutputHandler(EmailOutputConfig emailOutputConfig)
        {
            _emailOutputConfig = emailOutputConfig;

            _smtpClient = new SmtpClient(_emailOutputConfig.SmtpServerHost, _emailOutputConfig.SmtpServerPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailOutputConfig.FromEmail, _emailOutputConfig.FromEmailPassword)
            };
        }

        protected override void ProcessOutput(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            MailMessage mailMessage = new MailMessage(_emailOutputConfig.FromEmail, _emailOutputConfig.ToEmail, 
                GetMessageSubject(messageParameters, analysisParameters), GetMessageBody(messageParameters, analysisParameters));
             
            try
            {
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            { 
                Console.Write(ex.Message);
            }
        }

        private string GetMessageSubject(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            return "Logs Analyzer";
        }

        private string GetMessageBody(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            string formattedMessage = "";

            if (messageParameters?.Count() > 0)
            {
                for (int i = 0; i < messageParameters.Count(); i++)
                {
                    foreach (var parameter in messageParameters[i])
                    {
                        if (_emailOutputConfig.IncludedMessageParameters.ToList().Contains(parameter.Key))
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
                    if (_emailOutputConfig.IncludedAnalysisParameters?.ToList().Contains(parameter.Key) ?? true)
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

        public override void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}
