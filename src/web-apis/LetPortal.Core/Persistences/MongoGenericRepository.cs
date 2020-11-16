using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Core.Utils;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;

namespace LetPortal.Core.Persistences
{
    public class MongoGenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        protected MongoConnection Connection;

        private EntityCollectionAttribute entityCollectionAttribute = typeof(T).GetEntityCollectionAttribute();

        protected string CollectionName => entityCollectionAttribute.Name;

        protected IMongoCollection<T> Collection => Connection.GetDatabaseConnection().GetCollection<T>(CollectionName);

        protected IMongoCollection<TEntity> GetAnotherCollection<TEntity>() where TEntity : Entity
        {
            var collectionName = typeof(TEntity).GetEntityCollectionAttribute().Name;
            return Connection.GetDatabaseConnection().GetCollection<TEntity>(collectionName);
        }

        public async Task AddAsync(T entity)
        {
            entity.Check();
            if (string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = DataUtil.GenerateUniqueId();
            }
            if (entityCollectionAttribute.IsUniqueBackup)
            {
                await CheckIsExist(entity);
            }
            await Collection.InsertOneAsync(entity);
        }

        public async Task AddBulkAsync(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.Check();
                if (entityCollectionAttribute.IsUniqueBackup)
                {
                    await CheckIsExist(entity);
                }
            }
            var insertModels = entities.Select(a => new InsertOneModel<T>(a));
            await Collection.BulkWriteAsync(insertModels);
        }

        public async Task DeleteAsync(string id)
        {
            await Collection.DeleteOneAsync(a => a.Id == id);
        }

        public async Task DeleteBulkAsync(IEnumerable<string> ids)
        {
            var deleteModels = ids.Select(a => new DeleteOneModel<T>(Builders<T>.Filter.Eq(b => b.Id, a)));
            await Collection.BulkWriteAsync(deleteModels);
        }

        public IQueryable<T> GetAsQueryable()
        {
            return Collection.AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllByIdsAsync(
            IEnumerable<string> ids,
            Expression<Func<T, bool>> expression = null,
            bool isRequiredDiscriminator = false)
        {
            if (isRequiredDiscriminator)
            {
                if (ids != null && ids.Any())
                {
                    var builder = Collection.OfType<T>().AsQueryable().Where(a => ids.Contains(a.Id));
                    if (expression != null)
                    {
                        builder = builder.Where(expression);
                    }
                    return await builder.ToListAsync();
                }
            }
            else
            {
                if (ids != null && ids.Any())
                {
                    var builder = Collection.AsQueryable().Where(a => ids.Contains(a.Id));
                    if(expression != null)
                    {
                        builder = builder.Where(expression);
                    }
                    return await builder.ToListAsync();
                }
            }
            
            return null;
        }

        public async Task<T> GetOneAsync(string id)
        {
            return await Collection.AsQueryable().FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(string id, T entity)
        {
            entity.Check();
            if (entityCollectionAttribute.IsUniqueBackup)
            {
                await CheckIsExist(entity);
            }
            await Collection.FindOneAndReplaceAsync(a => a.Id == id, entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null, bool isRequiredDiscriminator = false)
        {
            if (isRequiredDiscriminator)
            {
                if (expression != null)
                {
                    return await Collection.OfType<T>().AsQueryable().Where(expression).ToListAsync();
                }

                return await Collection.OfType<T>().AsQueryable().ToListAsync();
            }
            else
            {
                if (expression != null)
                {
                    return await Collection.AsQueryable().Where(expression).ToListAsync();
                }

                return await Collection.AsQueryable().ToListAsync();
            }
        }

        private async Task CheckIsExist(T entity)
        {
            if (entity is BackupableEntity backupableEntity)
            {
                var backupableCollection = Connection.GetDatabaseConnection().GetCollection<BackupableEntity>(CollectionName);
                var isExist = await backupableCollection.AsQueryable().AnyAsync(a => a.Name == backupableEntity.Name && a.Id != backupableEntity.Id);
                if (isExist)
                {
                    throw new CoreException(ErrorCodes.NameAlreadyExistException);
                }
            }
        }

        public async Task<bool> IsExistAsync(Expression<Func<T, bool>> expression)
        {
            return await Collection.AsQueryable().AnyAsync(expression);
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

        public async Task ForceUpdateAsync(string id, T forceEntity)
        {
            var oldOne = await Collection.AsQueryable().FirstOrDefaultAsync(a => a.Id == id);
            if (oldOne != null)
            {
                await Collection.DeleteOneAsync(a => a.Id == id);
            }

            await Collection.InsertOneAsync(forceEntity);
        }

        public Task<T> FindAsync(Expression<Func<T, bool>> expression)
        {
            return Collection.AsQueryable().FirstAsync(expression);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Nothing to dispose with MongoDB
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
