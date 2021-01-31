using LogsManager.Common.Analyzer;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common
{
    public static class LogConditionHelper
    {
        public static bool DoesPassTheCondition(double parameterValue, string parameterName, AnalyzerConditionConfig condition)
        {
            bool passed = false;

            var conditionValue = double.Parse(condition.Value.ToString());

            if (condition != null)
            {
                switch (condition.CompareType)
                {
                    case Enums.ValueCompareTypes.EqualsTo:
                        passed = parameterValue == conditionValue;
                        break;
                    case Enums.ValueCompareTypes.HigherThan:
                        passed = parameterValue > conditionValue;
                        break;
                    case Enums.ValueCompareTypes.LessThan:
                        passed = parameterValue < conditionValue;
                        break;
                }
            }
            else
            {
                passed = true;
            }

            return passed;
        }
    }
}
