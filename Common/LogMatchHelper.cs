using System;
using System.Linq;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Enums;

namespace LogsManager.Common
{
    /// <summary>
    /// a helper class contains extension methods on LogMessageConfig class.
    /// </summary>
    public static class LogMatchHelper
    {
        /// <summary>
        /// returns whether a log message matches a specific message config.
        /// </summary>
        /// <param name="logMessageConfig"></param>
        /// <param name="logMessage"></param>
        /// <returns></returns>
        public static bool DoesMatch(this LogMessageConfig logMessageConfig, LogMessage logMessage)
        {
            bool doesMatch = true;

            logMessageConfig.LogMessageParameters.ToList()
                .ForEach(lm =>
                {
                    if (doesMatch)
                    {
                        switch (lm.FilterParameter)
                        {
                            case LogMessageParameters.Message:
                                doesMatch = DoesMessageMatch(lm.FilterValue, logMessage.Message, lm.TextCompareType);
                                break;
                            case LogMessageParameters.Level:
                                doesMatch = logMessage.LogLevel == (LogLevels)Enum.Parse(typeof(LogLevels), lm.FilterValue);
                                break;
                        }
                    }
                });

            return doesMatch;
        }

        /// <summary>
        /// returns whether a message matches a specific text filter or not.
        /// </summary>
        /// <param name="filterValue"></param>
        /// <param name="message"></param>
        /// <param name="textCompareType"></param>
        /// <returns></returns>
        private static bool DoesMessageMatch(string filterValue, string message, TextCompareTypes textCompareType)
        {
            return (textCompareType == TextCompareTypes.Contains && message.Contains(filterValue))
                    || (textCompareType == TextCompareTypes.StartsWith && message.StartsWith(filterValue))
                    || (textCompareType == TextCompareTypes.EndsWith && message.EndsWith(filterValue))
                    || (textCompareType == TextCompareTypes.Equals && message.Equals(filterValue));
        }

        /// <summary>
        /// returns whether two messages are the same or not.
        /// </summary>
        /// <param name="firstLogMessage"></param>
        /// <param name="secondLogMessage"></param>
        /// <returns></returns>
        public static bool IsTheSameMessage(this LogMessage firstLogMessage, LogMessage secondLogMessage)
        {
            return firstLogMessage.Message == secondLogMessage.Message;
        }
    }
}
