using LetPortal.Core.Logger;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Tests.ITs.Portal
{
    public class FakeServiceLogger<T> : IServiceLogger<T>
    {
        public void Critical(string message, params object[] logObjects)
        {
            
        }

        public void Critical(Exception exception, string message, params object[] logObjects)
        {
        }

        public void Debug(string message, params object[] logObjects)
        {
        }

        public void Debug(Exception exception, string message, params object[] logObjects)
        {                                       
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
        }

        public void Error(string message, params object[] logObjects)
        {
        }

        public void Error(Exception exception, string message, params object[] logObjects)
        {
        }

        public void Info(string message, params object[] logObjects)
        {
        }

        public void Info(Exception exception, string message, params object[] logObjects)
        {
        }

        public void Warning(string message, params object[] logObjects)
        {
        }

        public void Warning(Exception exception, string message, params object[] logObjects)
        {
        }
    }
}
