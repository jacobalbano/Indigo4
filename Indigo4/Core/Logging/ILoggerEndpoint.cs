using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Core.Logging
{
    public interface ILoggerEndpoint
    {
        void LogLine(LogLineInfo line);
    }

    public interface ILoggerEndpointConfig
    {
        ILoggerEndpoint InstantiateObject();
    }
}
