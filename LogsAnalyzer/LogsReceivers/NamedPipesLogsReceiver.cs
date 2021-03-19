using LogsManager.Common;
using LogsManager.Common.Analyzer;
using LogsManager.Common.Analyzer.Receivers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace LogsManager.Analyzer
{
    public class NamedPipesLogsReceiver : ILogsReceiver
    {
        NamedPipesReceiverConfig _logsReceiverConfig;

        NamedPipeClientStream _pipeClient;

        public NamedPipesLogsReceiver(NamedPipesReceiverConfig logsReceiverConfig)
        {
            _logsReceiverConfig = logsReceiverConfig;
        }

        public event EventHandler<LogMessage> OnNewLogMessage;

        public void Start()
        {
            Task.Run(() =>
            {
                _pipeClient = new NamedPipeClientStream(_logsReceiverConfig.PipeName);

                _pipeClient.Connect();

                using (StreamReader sr = new StreamReader(_pipeClient))
                {
                    // Display the read text to the console
                    string temp;

                    // Wait for 'sync message' from the server.
                    do
                    { 
                        temp = sr.ReadLine();

                        try
                        {
                            var logMessage = JsonConvert.DeserializeObject<LogMessage>(temp);

                            OnNewLogMessage?.Invoke(null, logMessage);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception?.Message);
                        }
                    }
                    while (!temp.StartsWith("@End@"));
                }

                Stop();
            });
        }

        public void Stop()
        {
            _pipeClient?.Dispose();
        }
    }
}
