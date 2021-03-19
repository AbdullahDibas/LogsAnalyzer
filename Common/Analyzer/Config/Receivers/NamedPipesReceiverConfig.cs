using LogsManager.Common.Enums;

namespace LogsManager.Common.Analyzer.Receivers
{
    /// <summary>
    /// a class represents the confiugations when Named Pipes are used to receive logs in the analyzer.
    /// </summary>
    public class NamedPipesReceiverConfig : LogsReceiverConfig
    {
        public NamedPipesReceiverConfig()
        {
            _logReceiverType = LogReceiverTypes.NamedPipes;
        }

        public string PipeName { get; set; }
    }
}
