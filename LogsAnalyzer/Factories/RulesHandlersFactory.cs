using LogsManager.Common.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

namespace LogsManager.Analyzer
{
    internal static class RuleHandlerFactory
    {
        private static string _analyzerAssemblyFileName = "LogsManager.Analyzer.dll";
        private static string _namespace = "LogsManager.Analyzer.Rules";

        public static Dictionary<int, IAnalyzerRuleHandler> CreateRulesHandlers(AnalyzerConfig analyzerConfig)
        {
            string anaylzerAssemblyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _analyzerAssemblyFileName);

            Dictionary<int, IAnalyzerRuleHandler> keyValuePairs = new Dictionary<int, IAnalyzerRuleHandler>();

            analyzerConfig.Rules.ToList().ForEach(r =>
            {
                if (r.IsEnabled)
                {
                    IAnalyzerRuleHandler analyzerRuleHandler = null;

                    if (File.Exists(anaylzerAssemblyFile))
                    {
                        var loggerAssembly = Assembly.LoadFrom(anaylzerAssemblyFile);

                        Type loggerType = loggerAssembly.GetType(_namespace + "." + r.AnalysisType.ToString() +"RuleHandler");

                        analyzerRuleHandler = (IAnalyzerRuleHandler)Activator.CreateInstance(loggerType, r.ID, r, analyzerConfig);
                    }

                    if (analyzerRuleHandler != null)
                    {
                        keyValuePairs.Add(r.ID, analyzerRuleHandler);
                    }
                }
            });

            return keyValuePairs;
        }
    }
}