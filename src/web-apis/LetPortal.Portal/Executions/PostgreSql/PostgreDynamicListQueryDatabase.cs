using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            throw new NotImplementedException();
        }
    }
}
