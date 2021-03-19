using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Receivers;
using System;
using System.IO;
using System.Reflection;

namespace LogsManager.Analyzer.Factories
{
    public static class LogsReceiverFactory
    {
        public static ILogsReceiver Create(string[] applicationArgs, AnalyzerConfig analyzerConfig)
        {
            ILogsReceiver logsReceiver = null;

            switch (analyzerConfig.LogsReceiverConfig.LogReceiverType)
            {
                case Common.Enums.LogReceiverTypes.AnonymousPipes:
                    if (applicationArgs?.Length > 0)
                    {
                        logsReceiver = new AnonymousPipesLogsReceiver(applicationArgs[0]);
                    }
                    break;
                case Common.Enums.LogReceiverTypes.NamedPipes:
                    logsReceiver = new NamedPipesLogsReceiver((NamedPipesReceiverConfig)analyzerConfig.LogsReceiverConfig);
                    break;
                case Common.Enums.LogReceiverTypes.WindowsEventsViewer:
                    string mircosoftEventViewerReceiverAssemblyFileName = "LogsManager.MircosoftEventViewerReceiverReceiver.dll";

                    IAnalyzerOutputHandler analyzerOutputHandler = null;

                    string assemblyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mircosoftEventViewerReceiverAssemblyFileName);

                    if (File.Exists(assemblyFileName))
                    {
                        var loggerAssembly = Assembly.LoadFrom(assemblyFileName);

                        Type outputHandlerType = loggerAssembly.GetType("LogsManager.MircosoftEventViewerReceiver.EventLogsReceiver");

                        analyzerOutputHandler = (IAnalyzerOutputHandler)Activator.CreateInstance(outputHandlerType, analyzerConfig.LogsReceiverConfig);
                    }

                    break;
            }

            return logsReceiver;
        }
    }
}
