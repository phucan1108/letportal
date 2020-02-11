using System;
using Serilog.Core;

namespace LetPortal.Core.Logger
{
    public interface IServiceLogger<T> : ILogEventEnricher
    {
        void Debug(string message, params object[] logObjects);

        void Debug(Exception exception, string message, params object[] logObjects);

        void Info(string message, params object[] logObjects);

        void Info(Exception exception, string message, params object[] logObjects);

        void Error(string message, params object[] logObjects);

        void Error(Exception exception, string message, params object[] logObjects);

        void Warning(string message, params object[] logObjects);

        void Warning(Exception exception, string message, params object[] logObjects);

        void Critical(string message, params object[] logObjects);

        void Critical(Exception exception, string message, params object[] logObjects);
    }
}
