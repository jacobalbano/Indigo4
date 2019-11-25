using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Core.Logging.Endpoints
{
    public class StringbuilderEndpoint : ILoggerEndpoint
    {
        public StringBuilder StringBuilder { get; }

        public StringbuilderEndpoint(StringBuilder sb)
        {
            StringBuilder = sb ?? throw new ArgumentNullException(nameof(sb));
        }

        public void LogLine(LogLineInfo line)
        {
            StringBuilder.AppendLine(line.ToString());
        }
    }
}
