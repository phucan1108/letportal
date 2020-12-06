using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger.Models;
using LetPortal.Core.Utils;
using LetPortal.Microservices.LogCollector;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Options;
using LetPortal.Microservices.Server.Repositories.Abstractions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LetPortal.Microservices.Server.Providers
{
    public class LogEventProvider : ILogEventProvider
    {
        private readonly ILogEventRepository _logEventRepository;

        private readonly IOptionsMonitor<CentralizedLogOptions> _options;

        public LogEventProvider(
            ILogEventRepository logEventRepository,
            IOptionsMonitor<CentralizedLogOptions> options)
        {
            _logEventRepository = logEventRepository;
            _options = options;
        }

        public async Task AddLogEvent(LogCollectorRequest logCollectorRequest)
        {
            var logEvent = new LogEvent
            {
                Id = DataUtil.GenerateUniqueId(),
                BeginRequest = logCollectorRequest.BeginRequest.ToDateTime(),
                EndRequest = logCollectorRequest.EndRequest.ToDateTime(),
                CpuUsage = logCollectorRequest.CpuUsage,
                ElapsedTime = logCollectorRequest.ElapsedTime,
                HttpRequestUrl = logCollectorRequest.HttpRequestUrl,
                HttpRequestHeaders = logCollectorRequest.HttpHeaders,
                HttRequestBody = logCollectorRequest.HttpRequestBody,
                HttpResponseStatusCode = logCollectorRequest.ResponseStatusCode,
                HttpResponseBody = logCollectorRequest.HttpRequestBody,
                MemoryUsed = logCollectorRequest.MemoryUsed,
                TraceId = logCollectorRequest.TraceId,
                Source = logCollectorRequest.ServiceName,
                SourceId = logCollectorRequest.ServiceId,
                StackTrace = logCollectorRequest.StackTraces
            };

            await _logEventRepository.AddAsync(logEvent);
        }

        public async Task GatherAllLogs(string traceId)
        {
            var allTraceLogs = await _logEventRepository.GetAllAsync(a => a.TraceId == traceId);
            if (allTraceLogs != null)
            {
                foreach (var trace in allTraceLogs)
                {
                    // Note: because of performance when we are using RDBMS to log
                    // So we only support MongoDB for main centralized log by now
                    // We will have a plan for other DBs in future 
                    switch (_options.CurrentValue.Database.ConnectionType)
                    {
                        case Core.Persistences.ConnectionType.MongoDB:
                            var databaseName = _options.CurrentValue.Database.Datasource;
                            var mongoDatabase = new MongoClient(_options.CurrentValue.Database.ConnectionString).GetDatabase(databaseName);
                            var mongoCollection = mongoDatabase.GetCollection<BsonDocument>(_options.CurrentValue.EntityLogName);
                            FieldDefinition<BsonDocument, string> traceIdField = "Properties.TraceId";
                            var traceIdFilter = Builders<BsonDocument>.Filter.Eq(traceIdField, traceId);
                            var logs = await mongoCollection.Find(traceIdFilter).ToListAsync();
                            if (logs != null)
                            {
                                var allStackTraces = from log in logs
                                                     select log["RenderedMessage"].AsString;

                                trace.StackTrace = allStackTraces;
                                await _logEventRepository.UpdateAsync(trace.Id, trace);
                            }
                            break;
                        case Core.Persistences.ConnectionType.SQLServer:
                            break;
                        case Core.Persistences.ConnectionType.PostgreSQL:
                            break;
                        case Core.Persistences.ConnectionType.MySQL:
                            break;
                    }
                }
            }
        }
    }
}
