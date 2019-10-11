using LetPortal.Core.Persistences;
using System;
using System.Linq.Expressions;

namespace LetPortal.Core.Versions
{
    public interface IVersionContext
    {
        ConnectionType ConnectionType { get; set; }

        object DatabaseOptions { get; set; }

        void DropAll<T>() where T : Entity;

        void BulkInsertData<T>(T[] entities) where T : Entity;

        void InsertData<T>(T entity) where T : Entity;

        void DeleteData<T>(string id) where T : Entity;

        void BulkDeleteData<T>(Expression<Func<T, bool>> expression) where T : Entity;

        void UpdateData<T>(string id, T entity) where T : Entity;
    }
}
