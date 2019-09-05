using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LetPortal.Core.Persistences
{
    public interface IGenericRepository<T> where T : Entity
    {
        Task<bool> IsExistAsync(string compareValue, string key = "name");

        Task AddAsync(T entity);

        Task UpdateAsync(string id, T entity);

        Task DeleteAsync(string id);

        IQueryable<T> GetAsQueryable();

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null);

        Task<IEnumerable<T>> GetAllByIdsAsync(List<string> ids);

        Task<T> GetOneAsync(string id);

        Task AddBulkAsync(IEnumerable<T> entities);

        Task DeleteBulkAsync(IEnumerable<string> ids);
    }
}
