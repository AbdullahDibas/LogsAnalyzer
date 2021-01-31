using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Analyzer.Factories
{
    internal static class LogsReceiverFactory
    {
        public static ILogsReceiver Create(string[] applicationArgs)
        {
            ILogsReceiver logsReceiver = null;

            if (applicationArgs?.Length > 0)
            {
                logsReceiver = new AnonymousPipesLogsReceiver(applicationArgs[0]);
            }

            return logsReceiver;
        }
    }
}
