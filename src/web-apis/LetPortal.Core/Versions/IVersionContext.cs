using System;
using System.Linq;
using System.Linq.Expressions;
using LetPortal.Core.Persistences;

namespace LetPortal.Core.Versions
{
    public interface IVersionContext
    {
        ConnectionType ConnectionType { get; set; }

        object DatabaseOptions { get; set; }

        object PortalDatabaseOptions { get; set; }

        object ServiceManagementOptions { get; set; }

        object IdentityDbOptions { get; set; }

        void ExecuteRaw(string rawCommand);

        void DropAll<T>() where T : Entity;

        void BulkInsertData<T>(T[] entities) where T : Entity;

        void InsertData<T>(T entity) where T : Entity;

        void DeleteData<T>(string id) where T : Entity;

        void BulkDeleteData<T>(Expression<Func<T, bool>> expression) where T : Entity;

        void UpdateData<T>(string id, T entity) where T : Entity;

        IQueryable<T> GetQueryable<T>() where T : Entity;

        void SaveChange();
    }
}
