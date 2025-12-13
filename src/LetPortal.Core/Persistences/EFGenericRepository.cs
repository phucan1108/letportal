using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
            foreach (var entity in entities)
            {
                entity.Check();
            }
            _context.Set<T>().AddRange(entities);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task<ComparisonResult> Compare(T comparedEntity)
        {
            var foundEntity = await GetOneAsync(comparedEntity.Id);
            if (foundEntity != null)
            {
                var jObject = JObject.FromObject(foundEntity);
                var comparedJObject = JObject.FromObject(comparedEntity);
                var children = jObject.Children();
                var comparedChildren = comparedJObject.Children();
                var result = new ComparisonResult
                {
                    Result = new ComparisonEntity { Properties = new List<ComparisonProperty>() }
                };
                foreach (JProperty jprop in comparedChildren)
                {
                    var foundChild = children.FirstOrDefault(a => (a as JProperty).Name == jprop.Name);
                    if (foundChild != null)
                    {
                        var resultProperty = new ComparisonProperty
                        {
                            Name = jprop.Name,
                            SourceValue = jprop.Value?.ToString(),
                            TargetValue = (foundChild as JProperty).Value?.ToString()
                        };

                        resultProperty.SourceValue = resultProperty.SourceValue ?? string.Empty;
                        resultProperty.TargetValue = resultProperty.TargetValue ?? string.Empty;

                        // In case the same string length, compare each char
                        if (resultProperty.SourceValue.Length == resultProperty.TargetValue.Length)
                        {
                            if (resultProperty.SourceValue.Length == 0)
                            {
                                resultProperty.ComparedState = ComparedState.Unchanged;
                            }
                            else
                            {
                                for (var i = 0; i < resultProperty.SourceValue.Length; i++)
                                {
                                    if (resultProperty.SourceValue[i] != resultProperty.TargetValue[i])
                                    {
                                        resultProperty.ComparedState = ComparedState.Changed;
                                        break;
                                    }
                                }

                                if (resultProperty.ComparedState != ComparedState.Changed)
                                {
                                    resultProperty.ComparedState = ComparedState.Unchanged;
                                }
                            }
                        }
                        else
                        {
                            resultProperty.ComparedState = ComparedState.Changed;
                        }

                        result.Result.Properties.Add(resultProperty);
                    }
                    else
                    {
                        result.Result.Properties.Add(
                            new ComparisonProperty
                            {
                                Name = jprop.Name,
                                SourceValue = jprop.Value?.ToString(),
                                ComparedState = ComparedState.New
                            });
                    }
                }

                result.IsUnchanged = result.Result.Properties.All(a => a.ComparedState == ComparedState.Unchanged);

                return result;
            }
            else
            {
                return new ComparisonResult { IsTotallyNew = true };
            }
        }

        public Task DeleteAsync(string id)
        {
            var entity = _context.Set<T>().Where(a => a.Id == id).First();
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task DeleteBulkAsync(IEnumerable<string> ids)
        {
            var dbSet = _context.Set<T>();
            var entities = dbSet.Where(a => ids.Contains(a.Id));
            foreach (var entity in entities)
            {
                dbSet.Remove(entity);
            }
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task ForceUpdateAsync(string id, T forceEntity)
        {
            var dbSet = _context.Set<T>();
            var deletedEntity = dbSet.FirstOrDefault(a => a.Id == id);
            if (deletedEntity != null)
            {
                dbSet.Remove(deletedEntity);
                _context.SaveChanges();
                dbSet.Add(forceEntity);
                _context.SaveChanges();
            }
            else
            {
                dbSet.Add(forceEntity);
                _context.SaveChanges();
            }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null, bool isRequiredDiscriminator = false)
        {
            if (expression != null)
            {
                var entities = _context.Set<T>().AsNoTracking().Where(expression);
                return Task.FromResult(entities.AsEnumerable());
            }
            else
            {
                var entities = _context.Set<T>();
                return Task.FromResult(entities.AsNoTracking().AsEnumerable());
            }
        }

        public Task<IEnumerable<T>> GetAllByIdsAsync(
            IEnumerable<string> ids,
            Expression<Func<T, bool>> expression = null,
            bool isRequiredDiscriminator = false)
        {
            if (ids != null || ids.Any())
            {
                var entities = _context.Set<T>().Where(a => ids.Contains(a.Id));
                if(expression != null)
                {
                    entities = entities.Where(expression);
                }
                return Task.FromResult(entities.AsEnumerable());
            }
            return null;
        }

        public IQueryable<T> GetAsQueryable()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public virtual Task<T> GetOneAsync(string id)
        {
            return Task.FromResult(_context.Set<T>().First(a => a.Id == id));
        }

        public Task<bool> IsExistAsync(Expression<Func<T, bool>> expression)
        {
            // We still got a problem EF Core for MySQL when using Any()
            // So that we keep using FirstOrDefault() for checking
            return Task.FromResult(_context.Set<T>().FirstOrDefault(expression) != null);
        }

        public Task UpdateAsync(string id, T entity)
        {
            entity.Check();
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> expression)
        {
            return Task.FromResult(_context.Set<T>().First(expression));
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
    }
}
