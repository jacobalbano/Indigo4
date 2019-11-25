using Indigo.Configuration;
using Indigo.Configuration.Modules.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Core.Logging.Endpoints
{
    public class ConsoleEndpoint : ILoggerEndpoint
    {
        [Module(typeof(DefaultModule))]
        public class Config : ConfigBase<ConsoleEndpoint>, ILoggerEndpointConfig
        {
            public override ConsoleEndpoint InstantiateObject()
            {
                return new ConsoleEndpoint();
            }

            ILoggerEndpoint ILoggerEndpointConfig.InstantiateObject() => InstantiateObject();
        }

        public void LogLine(LogLineInfo line)
        {
            Console.WriteLine(line.ToString());
        }

        public static Logger CreateLogger()
        {
            var result = new Logger();
            result.AddEndpoint(new ConsoleEndpoint());

            return result;
        }
    }
}
