using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Exceptions.Databases;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Services.Databases
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IEnumerable<IExecutionDatabase> _executionDatabases;

        private readonly IEnumerable<IExtractionDatabase> _extractionDatabases;

        public DatabaseService(
            IEnumerable<IExecutionDatabase> executionDatabases,
            IEnumerable<IExtractionDatabase> extractionDatabases
            )
        {
            _executionDatabases = executionDatabases;
            _extractionDatabases = extractionDatabases;
        }

        public async Task<ExecuteDynamicResultModel> ExecuteDynamic(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var connectionType = databaseConnection.GetConnectionType();
            var executionDatabase = _executionDatabases.FirstOrDefault(a => a.ConnectionType == connectionType);

            if (executionDatabase != null)
            {
                return await executionDatabase.Execute(databaseConnection, formattedString, parameters);
            }
            throw new DatabaseException(DatabaseErrorCodes.NotSupportedConnectionType);
        }

        public async Task<ExecuteDynamicResultModel> ExecuteDynamic(
            List<DatabaseConnection> databaseConnections,
            DatabaseExecutionChains executionChains,
            IEnumerable<ExecuteParamModel> parameters,
            IEnumerable<LoopDataParamModel> LoopDatas)
        {
            var context = new ExecutionDynamicContext
            {
                Data = JObject.Parse("{}")
            };

            var result = new ExecuteDynamicResultModel { IsSuccess = true };
            var parametersList = parameters.ToList();
            for (var i = 1; i <= executionChains?.Steps.Count; i++)
            {
                var step = executionChains.Steps[i - 1];
                var executingDatabaseConnection = databaseConnections.First(a => a.Id == step.DatabaseConnectionId);
                var connectionType =
                    executingDatabaseConnection.GetConnectionType();
                var executionDatabase = _executionDatabases.First(a => a.ConnectionType == connectionType);
                if (executionDatabase != null)
                {
                    step.ExecuteCommand = ReplaceValueWithContext(step.ExecuteCommand, context, ref parametersList, connectionType != Core.Persistences.ConnectionType.MongoDB);
                    StepExecutionResult stepResult;                    
                    // Single step
                    if (string.IsNullOrEmpty(step.DataLoopKey))
                    {
                        stepResult = await executionDatabase
                            .ExecuteStep(
                                executingDatabaseConnection, step.ExecuteCommand, parametersList, context);
                    }
                    else
                    {
                        if(LoopDatas != null)
                        {
                            var loopData = LoopDatas.First(a => a.Name == step.DataLoopKey);
                            var stepExecutionResults = new List<StepExecutionResult>();
                            foreach (var subParameters in loopData.Parameters)
                            {
                                var subStepResult = await executionDatabase
                                                .ExecuteStep(
                                                    executingDatabaseConnection, step.ExecuteCommand, subParameters, context);

                                stepExecutionResults.Add(subStepResult);
                            }

                            stepResult = new StepExecutionResult
                            {
                                IsSuccess = true,
                                Result = stepExecutionResults.Select(a => a.Result).ToArray(),
                                ExecutionType = StepExecutionType.Multiple
                            };
                        }
                        else
                        {
                            throw new DatabaseException(DatabaseErrorCodes.LoopDataIsNotNull);
                        }                                                       
                    }

                    if (stepResult.IsSuccess)
                    {
                        switch (stepResult.ExecutionType)
                        {
                            case StepExecutionType.Query:
                                WriteDataToContext($"step{i.ToString()}", ConvertUtil.SerializeObject(stepResult.Result, true), context);
                                break;
                            case StepExecutionType.Insert:
                                // Due to JSON .NET Serialize problem for dynamic properties isn't working with Camel cast
                                // We need to exchange a anonymous class (or object) before serializing
                                WriteDataToContext(
                                    $"step{i.ToString()}",
                                    connectionType == Core.Persistences.ConnectionType.MongoDB
                                        ? ConvertUtil.SerializeObject(new { stepResult.Result.Id }, true)
                                            : ConvertUtil.SerializeObject(stepResult.Result, true), context);
                                break;
                            case StepExecutionType.Update:
                                if (connectionType != Core.Persistences.ConnectionType.MongoDB && stepResult.Result != null)
                                {
                                    WriteDataToContext(
                                    $"step{i.ToString()}",
                                    ConvertUtil.SerializeObject(stepResult.Result, true), context);
                                }
                                break;
                            case StepExecutionType.Delete:
                                if (connectionType != Core.Persistences.ConnectionType.MongoDB && stepResult.Result != null)
                                {
                                    WriteDataToContext(
                                    $"step{i.ToString()}",
                                    ConvertUtil.SerializeObject(stepResult.Result, true), context);
                                }
                                break;
                            case StepExecutionType.Multiple:
                                if (stepResult.Result != null)
                                {
                                    WriteDataToContext(
                                    $"step{i.ToString()}",
                                    ConvertUtil.SerializeObject(stepResult.Result, true), context);
                                }
                                break;
                        }
                    }
                    else
                    {
                        return ExecuteDynamicResultModel.IsFailed(stepResult.Error);
                    }
                }
                else
                {
                    throw new DatabaseException(DatabaseErrorCodes.NotSupportedConnectionType);
                }
            }
            result.Result = context.Data.ToObject<dynamic>();
            return result;
        }

        public async Task<ExtractingSchemaQueryModel> ExtractColumnSchema(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var connectionType = databaseConnection.GetConnectionType();
            var extractionDatabase = _extractionDatabases.First(a => a.ConnectionType == connectionType);

            return await extractionDatabase.Extract(databaseConnection, formattedString, parameters);
        }

        private static string ReplaceValueWithContext(string str, ExecutionDynamicContext context, ref List<ExecuteParamModel> executeParamModels, bool isSql = false)
        {
            if (context == null || context.Data == null)
            {
                return str;
            }
            var allFields = StringUtil.GetByRegexMatchs(@"{{\$(.*?)}}", str, false);
            if (allFields.Length > 0)
            {
                foreach (var field in allFields)
                {
                    var replacedValue = context.Data.SelectToken(field);
                    if (replacedValue == null)
                    {
                        throw new ArgumentNullException($"Cannot found a replaced value in context. Name: {field}");
                    }

                    if (isSql)
                    {
                        var paramName = GetSqlParam(field, replacedValue.Type);
                        var executeParam = new ExecuteParamModel
                        {
                            Name = paramName,
                            ReplaceValue = replacedValue.Value<string>()
                        };

                        executeParamModels.Add(executeParam);
                        str = str.Replace("{{$" + field + "}}", "{{" + paramName + "}}", StringComparison.Ordinal);
                    }
                    else
                    {
                        switch (replacedValue.Type)
                        {
                            case JTokenType.Object:
                                str = str.Replace("\"{{$" + field + "}}\"", replacedValue.ToString(), StringComparison.Ordinal);
                                break;
                            case JTokenType.Boolean:
                            case JTokenType.Integer:
                                str = str.Replace("\"{{$" + field + "}}\"", replacedValue.Value<string>(), StringComparison.Ordinal);
                                break;
                            default:
                                str = str.Replace("{{$" + field + "}}", replacedValue.Value<string>(), StringComparison.Ordinal);
                                break;
                        }
                    }
                }

                return str;
            }
            else
            {
                return str;
            }
        }

        private static string GetSqlParam(string fieldName, JTokenType tokenType)
        {
            return fieldName + "|" + MapperConstants.ConvertFromJToken(tokenType);
        }

        private void WriteDataToContext(string name, string data, ExecutionDynamicContext context)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (context.Data.Children().Any(a => (a as JProperty).Name == name))
                {
                    var mergeObject = (JObject)context.Data[name];
                    var dataJObject = JObject.Parse(data);
                    mergeObject.Merge(dataJObject, new JsonMergeSettings
                    {
                        MergeArrayHandling = MergeArrayHandling.Merge,
                        MergeNullValueHandling = MergeNullValueHandling.Merge
                    });

                    context.Data.Remove(name);
                    context.Data.Add(name, mergeObject);
                }
                else
                {
                    context.Data.Add(name, JToken.Parse(data));
                }
            }
        }
    }
}
