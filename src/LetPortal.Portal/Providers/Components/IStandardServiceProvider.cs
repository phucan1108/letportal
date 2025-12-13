using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Portal.Providers.Components
{
    public interface IStandardServiceProvider
    {
        Task<IEnumerable<StandardComponent>> GetStandardComponentsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareStandardComponent(IEnumerable<StandardComponent> standardComponents);

        Task ForceUpdateStandards(IEnumerable<StandardComponent> standards);

        Task<IEnumerable<StandardComponent>> GetByAppId(string appId);

        Task DeleteAllByAppIdAsync(string appId);

        Task<bool> CheckStandardExist(Expression<Func<StandardComponent, bool>> expression);
    }
}
