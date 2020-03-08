using System;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace LetPortal.Core.Logger
{
    public class ServiceLogger<T> : IServiceLogger<T>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger _logger;

        public ServiceLogger(ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger.ForContext(typeof(T)).ForContext(this as ILogEventEnricher);
            _httpContextAccessor = httpContextAccessor;
        }

        public void Enrich(Serilog.Events.LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var traceId = StringUtil.DecodeBase64ToUTF8(_httpContextAccessor.HttpContext.Request.Headers[Constants.TraceIdHeader].ToString());
            var userSessionId = StringUtil.DecodeBase64ToUTF8(_httpContextAccessor.HttpContext.Request.Headers[Constants.UserSessionIdHeader].ToString());
            logEvent.AddPropertyIfAbsent(new LogEventProperty("TraceId", new ScalarValue(traceId)));
            logEvent.AddPropertyIfAbsent(new LogEventProperty("UserSessionId", new ScalarValue(userSessionId)));

        }

        public void Debug(string message, params object[] logObjects)
        {
            _logger.Debug(message, logObjects);
        }

        public void Debug(Exception exception, string message, params object[] logObjects)
        {
            _logger.Debug(exception, message, logObjects);
        }

        public void Info(string message, params object[] logObjects)
        {
            _logger.Information(message, logObjects);
        }

        public void Info(Exception exception, string message, params object[] logObjects)
        {
            _logger.Information(exception, message, logObjects);
        }

        public void Error(string message, params object[] logObjects)
        {
            _logger.Error(message, logObjects);
        }

        public void Error(Exception exception, string message, params object[] logObjects)
        {
            _logger.Error(exception, message, logObjects);
        }

        public void Warning(string message, params object[] logObjects)
        {
            _logger.Warning(message, logObjects);
        }

        public void Warning(Exception exception, string message, params object[] logObjects)
        {
            _logger.Warning(exception, message, logObjects);
        }

        public void Critical(string message, params object[] logObjects)
        {
            _logger.Fatal(message, logObjects);
        }

        public void Critical(Exception exception, string message, params object[] logObjects)
        {
            _logger.Fatal(exception, message, logObjects);
        }
    }
}
