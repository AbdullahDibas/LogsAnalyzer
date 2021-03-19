using LogsManager.Common.Enums;

namespace LogsManager.Common.Analyzer.Receivers
{
    /// <summary>
    /// a class represents the confiugations when the Events Viewer in Windows OS is being used as a logs source for the analyzer.
    /// </summary>
    public class WindowsEventsViewerReceiverConfig : LogsReceiverConfig
    {
        public WindowsEventsViewerReceiverConfig()
        {
            _logReceiverType = LogReceiverTypes.WindowsEventsViewer;
        }

        public string MachineHost { get; set; }

        public string LogsCategoryName { get; set; }
    }
}
