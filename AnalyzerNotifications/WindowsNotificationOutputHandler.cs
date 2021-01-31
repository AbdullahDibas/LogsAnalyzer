using LogsManager.Common.Analyzer;
using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace LogsManager.Analyzer.Outputs
{
    public class WindowsNotificationOutputHandler : IAnalyzerOutputHandler
    {
        private LogMessageParameters[] _includedMessageParameters;
        private string[] _includedAnalysisParameters;
        private NotifyIcon _notifyIcon = new NotifyIcon();

        public int OutputID { get; set; }

        public WindowsNotificationOutputHandler(AnalyzerOutputConfig analyzerOutputConfig)
        {
            _includedMessageParameters = analyzerOutputConfig.IncludedMessageParameters;

            _includedAnalysisParameters = analyzerOutputConfig.IncludedAnalysisParameters; 
        }

        public void Dispose()
        {
        }

        public void Output(Dictionary<LogMessageParameters, string>[] messageParameters, Dictionary<string, string> analysisParameters)
        {
            string formattedMessage = "";

            if (messageParameters?.Count() > 0)
            {
                for (int i = 0; i < messageParameters.Count(); i++)
                {
                    foreach (var parameter in messageParameters[i])
                    {
                        if (_includedMessageParameters.ToList().Contains(parameter.Key))
                        {
                            formattedMessage += parameter.Key.ToString() + " : " + parameter.Value;
                            formattedMessage += Environment.NewLine;
                        }
                    }

                    if (!string.IsNullOrEmpty(formattedMessage))
                    {
                        formattedMessage += Environment.NewLine;
                    }
                }
            }

            if (analysisParameters?.Count > 0)
            {
                foreach (var parameter in analysisParameters)
                {
                    if (_includedAnalysisParameters?.ToList().Contains(parameter.Key) ?? true)
                    {
                        formattedMessage += parameter.Key.ToString() + " : " + parameter.Value;
                        formattedMessage += Environment.NewLine;
                    }
                }
            }

            ShowNotification(formattedMessage, "Logs Analyzer");
        }

        private void ShowNotification(string message, string title)
        {
            _notifyIcon.Visible = true;

            _notifyIcon.BalloonTipTitle = title;

            _notifyIcon.BalloonTipText = message;

            _notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;

            _notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            _notifyIcon.ShowBalloonTip(30000);
        }

        private void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
             
        }
    }
}
