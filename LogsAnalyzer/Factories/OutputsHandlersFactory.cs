using LogsManager.Common.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace LogsManager.Analyzer
{
    internal static class OutputsHandlersFactory
    {
        private static string _analyzerAssemblyFileName = "LogsManager.Analyzer.dll";

        private static string _namespace = "LogsManager.Analyzer.Outputs";

        public static Dictionary<int, IAnalyzerOutputHandler> GetOutputsHandlers(AnalyzerConfig analyzerConfig)
        {
            Dictionary<int, IAnalyzerOutputHandler> keyValuePairs = new Dictionary<int, IAnalyzerOutputHandler>();

            analyzerConfig.Outputs.ToList().ForEach(o =>
            { 
                IAnalyzerOutputHandler analyzerOutputHandler = null;
               
                string assemblyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _analyzerAssemblyFileName);

                if (o.OutputType == Common.Enums.OutputTypes.WindowsNotification)
                {
                    assemblyFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AnalyzerNotifications.dll");
                }

                if (File.Exists(assemblyFileName))
                {        
                    var loggerAssembly = Assembly.LoadFrom(assemblyFileName);

                    Type outputHandlerType = loggerAssembly.GetType(_namespace + "." + o.OutputType.ToString() + "OutputHandler");

                    analyzerOutputHandler = (IAnalyzerOutputHandler)Activator.CreateInstance(outputHandlerType, o); 
                }

                if (analyzerOutputHandler != null)
                {
                    keyValuePairs.Add(o.ID, analyzerOutputHandler);
                } 
            });

            return keyValuePairs;
        }
    }
}
