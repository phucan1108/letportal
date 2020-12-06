using System;
using System.Linq;
using System.Linq.Expressions;
using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Core.Versions
{
    public class EFVersionContext : IVersionContext, IDisposable
    {
        private readonly DbContext _context;

        public EFVersionContext(DbContext context)
        {
            _context = context;
        }

        public ConnectionType ConnectionType { get; set; }
        public object DatabaseOptions { get; set; }

        public object PortalDatabaseOptions { get; set; }

        public object ServiceManagementOptions { get; set; }
        public object IdentityDbOptions { get; set; }

        public void BulkDeleteData<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            var dbSet = _context.Set<T>();
            var allEntities = dbSet.AsNoTracking().Where(expression).AsEnumerable();
            dbSet.RemoveRange(allEntities);
            _context.SaveChanges();
        }

        public void BulkInsertData<T>(T[] entities) where T : Entity
        {
            foreach (var entity in entities)
            {
                entity.Check();
            }
            var dbSet = _context.Set<T>();
            dbSet.AddRange(entities);
            _context.SaveChanges();
        }

        public void DeleteData<T>(string id) where T : Entity
        {
            var entity = GetById<T>(id);
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }

        public IQueryable<T> GetQueryable<T>() where T : Entity
        {
            return _context.Set<T>().AsNoTracking().AsQueryable();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public void DropAll<T>() where T : Entity
        {
            // Do nothing
        }

        public void ExecuteRaw(string rawCommand)
        {
            var dbTransaction = _context.Database.BeginTransaction();
            try
            {
                _context.Database.ExecuteSqlRaw(rawCommand);
                dbTransaction.Commit();
            }
            catch
            {
                dbTransaction.Rollback();
                throw;
            }
        }

        public void InsertData<T>(T entity) where T : Entity
        {
            entity.Check();
            var dbSet = _context.Set<T>();
            dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void SaveChange()
        {
            _context.SaveChanges();
        }

        public void UpdateData<T>(string id, T entity) where T : Entity
        {
            entity.Check();
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        private T GetById<T>(string id) where T : Entity
        {
            return _context.Set<T>().AsNoTracking().First(a => a.Id == id);
        }
    }
}
