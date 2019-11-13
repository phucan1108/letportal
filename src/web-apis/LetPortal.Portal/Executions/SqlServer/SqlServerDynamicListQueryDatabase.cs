﻿using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Models.DynamicLists;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        private readonly IDynamicQueryBuilder _builder;

        private readonly ISqlServerMapper _sqlServerMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public SqlServerDynamicListQueryDatabase(
            IDynamicQueryBuilder builder,
            ISqlServerMapper sqlServerMapper,
            ICSharpMapper cSharpMapper)
        {
            _builder = builder;
            _sqlServerMapper = sqlServerMapper;
            _cSharpMapper = cSharpMapper;
        }

        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var response = new DynamicListResponseDataModel();
            using(var sqlDbConnection = new SqlConnection(databaseConnection.ConnectionString))
            {
                var combinedQuery =
                    _builder
                        .Init(
                            dynamicList.ListDatasource.DatabaseConnectionOptions.Query,
                            fetchDataModel.FilledParameterOptions.FilledParameters,
                            options =>
                            {
                                options.ContainsOperatorFormat = " LIKE '%' + {0} + '%'";
                                options.PaginationFormat = "OFFSET {1} ROWS FETCH NEXT {0} ROWS ONLY";
                                options.FieldFormat = "[{0}]";
                                options.DateCompareFormat = "cast({0} as date){1}cast({2} as date)";
                            })
                        .AddTextSearch(fetchDataModel.TextSearch, dynamicList.ColumnsList.ColumndDefs.Where(a => a.SearchOptions.AllowTextSearch).Select(b => b.Name))
                        .AddFilter(fetchDataModel.FilterGroupOptions.FilterGroups)
                        .AddSort(fetchDataModel.SortOptions.SortableFields)
                        .AddPagination(fetchDataModel.PaginationOptions.PageNumber, fetchDataModel.PaginationOptions.PageSize)
                        .Build();

                sqlDbConnection.Open();
                using(var cmd = new SqlCommand(combinedQuery.CombinedQuery, sqlDbConnection))
                {
                    foreach(var param in combinedQuery.Parameters)
                    {
                        if(param.IsReplacedValue)
                        {
                            var castObject = _cSharpMapper.GetCSharpObjectByType(param.Value, param.ReplaceValueType);
                            cmd.Parameters.Add(
                                new SqlParameter(
                                    param.Name, _sqlServerMapper.GetSqlDbType(param.ReplaceValueType))
                                {
                                    Value = castObject,
                                    Direction = System.Data.ParameterDirection.Input
                                });
                        }
                        else
                        {
                            cmd.Parameters.Add(
                              new SqlParameter(
                                  param.Name, GetSqlDbType(param, out object castObject))
                              {
                                  Value = castObject,
                                  Direction = System.Data.ParameterDirection.Input
                              });
                        }                        
                    }
                    using(var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        response.Data = ConvertUtil.DeserializeObject<dynamic>(ConvertUtil.SerializeObject(dt, true));
                    }
                }
                sqlDbConnection.Close();
            }

            return Task.FromResult(response);
        }

        private SqlDbType GetSqlDbType(DynamicQueryParameter param, out object castObj)
        {
            switch(param.ValueType)
            {
                case FieldValueType.Number:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Long);
                    return SqlDbType.Int;
                case FieldValueType.DatePicker:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Date);
                    return SqlDbType.Date;
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Bool);
                    return SqlDbType.Bit;
                case FieldValueType.Select:
                case FieldValueType.Text:
                default:
                    castObj = param.Value;
                    return SqlDbType.NVarChar;
            }
        }
    }
}