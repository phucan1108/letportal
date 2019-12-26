using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LetPortal.Core.Persistences
{
    public interface IGenericRepository<T> where T : Entity
    {      
        Task<bool> IsExistAsync(Expression<Func<T, bool>> expression);

        Task AddAsync(T entity);

        Task UpdateAsync(string id, T entity);

        Task ForceUpdateAsync(string id, T forceEntity);

        Task DeleteAsync(string id);

        IQueryable<T> GetAsQueryable();

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null, bool isRequiredDiscriminator = false);

        Task<IEnumerable<T>> GetAllByIdsAsync(IEnumerable<string> ids);

        Task<T> GetOneAsync(string id);

        Task AddBulkAsync(IEnumerable<T> entities);

        Task DeleteBulkAsync(IEnumerable<string> ids);

        Task<ComparisonResult> Compare(T comparedEntity);
    }
}
