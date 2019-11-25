using Indigo.Configuration.Modules.Default;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo.Configuration;

namespace Indigo.Core.Logging
{
    public sealed class Logger
    {
        public bool IncludeCallingMethod { get; set; }

        [Module(typeof(DefaultModule))]
        public class Config : ConfigBase<Logger>
        {
            public bool IncludeCallingMethod { get; set; }

            public List<ILoggerEndpointConfig> Endpoints { get; set; } = new List<ILoggerEndpointConfig>();

            public override Logger InstantiateObject()
            {
                var logger = new Logger() { IncludeCallingMethod = IncludeCallingMethod };
                foreach (var c in Endpoints)
                    logger.AddEndpoint(c.InstantiateObject());
                return logger;
            }
        }

        public Logger()
        {
            endpoints = new List<ILoggerEndpoint>();
            contexts = new Stack<LoggerContext>();
        }

        public void AddEndpoint(ILoggerEndpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            endpoints.Add(endpoint);
        }

        public void WriteLine()
        {
            var lineInfo = new LogLineInfo { Depth = contexts.Count };

            if (IncludeCallingMethod)
            {
                var trace = new StackTrace();
                for (int i = 0; i < trace.FrameCount; i++)
                {
                    var method = trace.GetFrame(i).GetMethod();
                    if (method.DeclaringType != typeof(Logger))
                    {
                        lineInfo.CallingClass = method.DeclaringType.Name;
                        lineInfo.CallingMethod = (method.IsConstructor ? lineInfo.CallingClass : method.Name);
                        break;
                    }
                }

            }
            
            lineInfo.Message = "";

            foreach (var ep in endpoints)
                ep.LogLine(lineInfo);
        }

        public void WriteLine(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            var lineInfo = new LogLineInfo { Depth = contexts.Count };

            if (IncludeCallingMethod)
            {
                var trace = new StackTrace();
                for (int i = 0; i < trace.FrameCount; i++)
                {
                    var method = trace.GetFrame(i).GetMethod();
                    if (method.DeclaringType != typeof(Logger))
                    {
                        lineInfo.CallingClass = method.DeclaringType.Name;
                        lineInfo.CallingMethod = (method.IsConstructor ? lineInfo.CallingClass : method.Name);
                        break;
                    }
                }
            }

            var lines = message.Split(crlf, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                lineInfo.Message = line;

                foreach (var ep in endpoints)
                    ep.LogLine(lineInfo);
            }
        }

        #region WriteLine overrides
        public void WriteLine<T>(T obj)
        {
            WriteLine(obj?.ToString());
        }

        public void WriteLine<T0>(string format, T0 arg0)
        {
            WriteLine(string.Format(format, arg0));
        }

        public void WriteLine<T0, T1>(string format, T0 arg0, T1 arg1)
        {
            WriteLine(string.Format(format, arg0, arg1));
        }

        public void WriteLine<T0, T1, T2>(string format, T0 arg0, T1 arg1, T2 arg2)
        {
            WriteLine(string.Format(format, arg0, arg1, arg2));
        }

        public void WriteLine<T0, T1, T2>(string format, params object[] args)
        {
            WriteLine(string.Format(format, args));
        }

        #endregion

        #region Context
        public IDisposable Context(string description)
        {
            WriteLine(description);

            var ctx = new LoggerContext(this);
            contexts.Push(ctx);
            return ctx;
        }

        public void Context<T0>(string format, T0 arg0)
        {
            Context(string.Format(format, arg0));
        }

        public void Context<T0, T1>(string format, T0 arg0, T1 arg1)
        {
            Context(string.Format(format, arg0, arg1));
        }

        public void Context<T0, T1, T2>(string format, T0 arg0, T1 arg1, T2 arg2)
        {
            Context(string.Format(format, arg0, arg1, arg2));
        }

        public void Context<T0, T1, T2>(string format, params object[] args)
        {
            Context(string.Format(format, args));
        }
        
        private void PopContext()
        {
            contexts.Pop();
        }

        private class LoggerContext : IDisposable
        {
            private readonly Logger Host;

            public LoggerContext(Logger host)
            {
                Host = host;
            }

            public void Dispose()
            {
                Host.PopContext();
            }
        }

        #endregion

        private List<ILoggerEndpoint> endpoints;
        private Stack<LoggerContext> contexts;

        private static readonly char[] crlf = "\r\n".ToCharArray();
    }
}
