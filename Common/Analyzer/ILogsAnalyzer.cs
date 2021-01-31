namespace LogsManager.Common
{
    public interface ILogsAnalyzer
    {
        void Start();

        void AnalyzeLog(LogMessage logMessage);

        void Dispose();
    }
}
