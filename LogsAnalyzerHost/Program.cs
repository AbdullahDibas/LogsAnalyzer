using System;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Analyzer;
using LogsManager.Analyzer.Factories;
using System.Collections.Generic;

namespace LogsManager.AnalyzerHost
{
    public class Program
    { 
        private static ILogsAnalyzer _logsAnalyzer;

        public static void Main(string[] args)
        {
            try
            {
                AnalyzerConfig analyzerConfig = ConfigurationsFactory.GetAnalyzerConfig();

                if (analyzerConfig?.IsEnabled ?? false)
                {
                    ILogsReceiver logsReceiver = LogsReceiverFactory.Create(args, analyzerConfig);

                    Dictionary<int, IAnalyzerRuleHandler> analyzerRuleHandlers = RuleHandlerFactory.CreateRulesHandlers(analyzerConfig);

                    Dictionary<int, IAnalyzerOutputHandler> analyzerOutputHandlers = OutputsHandlersFactory.GetOutputsHandlers(analyzerConfig);

                    Dictionary<int, IAnalyzerScheduleHandler> analyzerScheduleHandlers = SchedulesHandlersFactory.CreateSchedulesHandlers(analyzerConfig);

                    _logsAnalyzer = new LogsAnalyzer(analyzerConfig, logsReceiver, analyzerRuleHandlers, analyzerOutputHandlers, analyzerScheduleHandlers);

                    _logsAnalyzer.Start();
                } 

                Console.Write("Press enter to exit...");

                Console.ReadLine();

                _logsAnalyzer.Dispose();

                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Error:");

                Console.WriteLine(exception?.Message);
            }
        }
    }
}
