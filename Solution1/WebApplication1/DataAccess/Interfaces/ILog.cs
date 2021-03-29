using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.DataAccess.Interfaces
{
    public interface ILog
    {
        void Log(string message,  Google.Cloud.Logging.Type.LogSeverity severity);
    }
}
