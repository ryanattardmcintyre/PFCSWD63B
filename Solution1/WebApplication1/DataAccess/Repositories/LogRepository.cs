using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.DataAccess.Interfaces;
using Google.Cloud.Logging.V2;
using Google.Api;

namespace WebApplication1.DataAccess.Repositories
{
    public class LogRepository : ILog
    {
        public void Log(string message, Google.Cloud.Logging.Type.LogSeverity severity )
        {
            var loggingClient = LoggingServiceV2Client.Create();

            LogName logName = new LogName("pfc2021", "pfc2021swd63b"); //log id

            MonitoredResource res = new MonitoredResource();
            res.Type = "global"; //global its like a category under which the entry that you will log will be classifed under

            List<LogEntry> entries = new List<LogEntry>();
            entries.Add(new LogEntry() { LogName = logName.ToString(), Severity = severity, TextPayload = message });

            loggingClient.WriteLogEntries(logName, res, null, entries);
        }
    }
}
