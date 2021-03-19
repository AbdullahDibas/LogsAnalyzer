using LogsManager.Common.Enums;

namespace LogsManager.Common.Analyzer
{
    /// <summary>
    /// a class represents the configurations of what receives logs in the analyzer.
    /// </summary>
    public class LogsReceiverConfig
    {
        protected LogReceiverTypes _logReceiverType;

        public LogReceiverTypes LogReceiverType 
        {
            get
            {
                return _logReceiverType; 
            } 
            set 
            { 
                _logReceiverType = value; 
            }
        }
    }
}
