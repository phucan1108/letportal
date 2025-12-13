using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components.Controls;

namespace LetPortal.Portal.Providers.CompositeControls
{
    public interface ICompositeControlServiceProvider
    {
        Task<IEnumerable<CompositeControl>> GetByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> Compare(IEnumerable<CompositeControl> compositeControl);

        Task ForceUpdate(IEnumerable<CompositeControl> compositeControls);
    }
}
