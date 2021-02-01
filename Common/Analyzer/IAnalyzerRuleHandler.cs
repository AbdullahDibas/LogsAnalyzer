using System;

namespace LogsManager.Common.Analyzer
{
    public interface IAnalyzerRuleHandler
    {
        int RuleID { get; }

        void HandleLog(LogMessage logMessage);


        event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        void Dispose();
    }
}
