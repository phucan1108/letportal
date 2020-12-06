using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.Core.Persistences
{
#pragma warning disable CA1063 // Implement IDisposable Correctly
    public class InMemoryGenericRepository<T> : IGenericRepository<T> where T : Entity
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
        protected virtual string CacheKey { get; }

        private readonly IMemoryCache _memoryCache;

        protected List<T> List { get; set; }

        public InMemoryGenericRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _memoryCache.TryGetValue(CacheKey, out List<T> list);
            List = list;
        }

        public Task AddAsync(T entity)
        {
            List.Add(entity);
            return Task.CompletedTask;
        }

        public Task AddBulkAsync(IEnumerable<T> entities)
        {
            List.AddRange(entities);
            return Task.CompletedTask;
        }

        public Task<ComparisonResult> Compare(T comparedEntity)
        {
            var foundEntity = List.FirstOrDefault(a => a.Id == comparedEntity.Id);
            return Task.FromResult(new ComparisonResult());
        }

        public Task DeleteAsync(string id)
        {
            List.Remove(List.Find(a => a.Id == id));
            return Task.CompletedTask;
        }

        public async Task DeleteBulkAsync(IEnumerable<string> ids)
        {
            foreach (var id in ids)
            {
                await DeleteAsync(id);
            }
        }

        public void Dispose()
        {
            // Do nothing
        }

        public Task ForceUpdateAsync(string id, T forceEntity)
        {
            var found = List.First(a => a.Id == id);
            List.Remove(found);
            List.Add(forceEntity);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>> expression = null, bool isRequiredDiscriminator = false)
        {
            return Task.FromResult(List.AsQueryable().Where(expression).AsEnumerable());
        }

        public Task<IEnumerable<T>> GetAllByIdsAsync(IEnumerable<string> ids, System.Linq.Expressions.Expression<Func<T, bool>> expression = null, bool isRequiredDiscriminator = false)
        {
            return Task.FromResult(List.AsQueryable().Where(a => ids.Any(b => b == a.Id)).Where(expression).AsEnumerable());
        }

        public System.Linq.IQueryable<T> GetAsQueryable()
        {
            return List.AsQueryable();
        }

        public Task<T> GetOneAsync(string id)
        {
            return Task.FromResult(List.First(a => a.Id == id));
        }

        public Task<bool> IsExistAsync(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return Task.FromResult(List.AsQueryable().Any(expression));
        }

        public Task UpdateAsync(string id, T entity)
        {
            var found = List.Find(a => a.Id == id);
            found = entity;
            return Task.CompletedTask;
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> expression)
        {                                                                        
            return Task.FromResult(List.First(expression.Compile()));
        }
    }
}
