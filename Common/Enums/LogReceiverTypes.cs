using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Enums
{
    public enum LogReceiverTypes
    {
        None = 0,
        AnonymousPipes = 1,
        NamedPipes = 2,
        WindowsEventsViewer = 3
    }
}
