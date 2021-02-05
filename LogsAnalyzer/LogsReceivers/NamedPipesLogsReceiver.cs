using LogsManager.Common;
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
        string _pipeHandle;

        NamedPipeClientStream _pipeClient;

        public NamedPipesLogsReceiver(string pipeHandle)
        {
            _pipeHandle = pipeHandle;
        }

        public event EventHandler<LogMessage> OnNewLogMessage;

        public void Start()
        {
            Task.Run(() =>
            {
                _pipeClient = new NamedPipeClientStream("LogsAnalyzerPipe");

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
