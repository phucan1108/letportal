using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LetPortal.Core.Persistences
{
    public class EFGenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        private readonly DbContext _context;

        public EFGenericRepository(DbContext context)
        {
            _context = context;
        }

        public Task AddAsync(T entity)
        {
            entity.Check();
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task AddBulkAsync(IEnumerable<T> entities)
        {
            foreach(var entity in entities)
            {
                entity.Check();
            }
            _context.Set<T>().AddRange(entities);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string id)
        {
            var entity = _context.Set<T>().AsNoTracking().Where(a => a.Id == id).First();
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task DeleteBulkAsync(IEnumerable<string> ids)
        {
            var entities = _context.Set<T>().AsNoTracking().Where(a => ids.Contains(a.Id));
            _context.Set<T>().RemoveRange(entities);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null, bool isRequiredDiscriminator = false)
        {
            if(expression != null)
            {
                var entities = _context.Set<T>().Where(expression);
                return Task.FromResult(entities.AsEnumerable());
            }
            else
            {
                var entities = _context.Set<T>();
                return Task.FromResult(entities.AsEnumerable());
            }
        }

        public Task<IEnumerable<T>> GetAllByIdsAsync(List<string> ids)
        {
            var entities = _context.Set<T>().Where(a => ids.Contains(a.Id));
            return Task.FromResult(entities.AsEnumerable());
        }

        public IQueryable<T> GetAsQueryable()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public Task<T> GetOneAsync(string id)
        {
            return Task.FromResult(_context.Set<T>().First(a => a.Id == id));
        }

        public Task<bool> IsExistAsync(Expression<Func<T, bool>> expression)
        {
            return Task.FromResult(_context.Set<T>().AsNoTracking().Any(expression));
        }

        public Task UpdateAsync(string id, T entity)
        {
            entity.Check();
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
