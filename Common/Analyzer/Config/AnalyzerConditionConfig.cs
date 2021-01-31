using LogsManager.Common.Enums;

namespace LogsManager.Common.Analyzer
{
    public class AnalyzerConditionConfig
    {
        public int ID { get; set; }

        public string ParamName { get; set; }

        public ValueCompareTypes CompareType { get; set; }

        public object Value { get; set; }
    }
}
