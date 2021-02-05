using System;
using System.Collections.Generic;
using System.Linq;
using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Rules;
using LogsManager.Common.Enums;

namespace LogsManager.Analyzer.Rules
{
    /// <summary>
    /// represents a log rule that searches for matches of a specific sequence of logs.
    /// </summary>
    public class AggregateFunctionRuleHandler : IAnalyzerRuleHandler
    {
        public int RuleID { get; set; }

        /// <summary>
        /// the total count of the received messages that match the configurations.
        /// </summary>
        private int _messagesCount = 0;

        /// <summary>
        /// the total value of the parameter passed in the received messages that match the configurations.
        /// </summary>
        private double _totalValue = 0;

        /// <summary>
        /// the ouptut value calculated from the total value and the aggregate function.
        /// </summary>
        private double _aggregatedValue = 0;

        /// <summary>
        /// the configurations that represent the messages to be processed.
        /// </summary>
        private readonly LogMessageConfig _logMessage;

        private readonly AggregateFunctionRule _aggregateFunctionRule;

        private readonly AnalyzerConditionConfig[] _conditions;

        public event EventHandler<AnalyzerResultEventArgs> OnAnalyzerResult;

        public AggregateFunctionRuleHandler(int id, AggregateFunctionRule aggregateFunctionRule, AnalyzerConfig analyzerConfig)
        {
            RuleID = id;

            _aggregateFunctionRule = aggregateFunctionRule;

            _logMessage = analyzerConfig.LogMessages.FirstOrDefault(lm => lm != null && _aggregateFunctionRule.LogMessageID == lm.ID);

            _conditions = aggregateFunctionRule.ConditionsIDs?.Select(cID => analyzerConfig.Conditions?.FirstOrDefault(c => c.ID == cID)).ToArray();
        }

        /// <summary>
        /// handles and analyzes the received log message. 
        /// </summary>
        /// <param name="logMessage"></param>
        public void HandleLog(LogMessage logMessage)
        {
            if ((logMessage.Parameters?.Any(par => par.Key == _aggregateFunctionRule.ParamName)?? false ||
                _aggregateFunctionRule.AggregateFunction == AggregateFunctions.Count) && LogMatchHelper.DoesMatch(_logMessage, logMessage))
            {
                _messagesCount++;

                SetAggregatedValue(logMessage.Parameters.FirstOrDefault(par => par.Key == _aggregateFunctionRule.ParamName).Value);

                AnalyzerResultEventArgs eventArgs = new AnalyzerResultEventArgs
                {
                    RuleID = RuleID,
                    AnalysisParameters = new Dictionary<string, string>
                        {
                            {
                                "Value", _aggregatedValue.ToString()
                            }
                        },
                    Messages = new LogMessage[]  { logMessage }
                };

                if (DoessPassConditions(_aggregatedValue, "Value"))
                {
                    OnAnalyzerResult.Invoke(this, eventArgs);
                }
            }
        }

        private bool DoessPassConditions(double parameterValue, string parameterName)
        {
            bool passed = false;

            if (_aggregateFunctionRule.ConditionsIDs?.Count() > 0)
            {
                return _conditions.All(c => LogConditionHelper.DoesPassTheCondition(parameterValue, parameterName, c));
            }
            else
            {
                passed = true;
            }

            return passed;
        }

        private void SetAggregatedValue(string paramValueString)
        {
            if (double.TryParse(paramValueString, out double paramValue))
            {
                switch (_aggregateFunctionRule.AggregateFunction)
                {
                    case AggregateFunctions.Count:
                        _aggregatedValue = _messagesCount;
                        break;
                    case AggregateFunctions.Sum:
                        _totalValue += paramValue;
                        _aggregatedValue = _totalValue;
                        break;
                    case AggregateFunctions.Average:
                        _totalValue += paramValue;
                        _aggregatedValue = _totalValue / _messagesCount;
                        break;
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
