using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Repositories.Components;

namespace LetPortal.Portal.Providers.CompositeControls
{
    public class InternalCompositeControlProvider : ICompositeControlServiceProvider
    {
        private readonly ICompositeControlRepository _compositeControlRepository;

        public InternalCompositeControlProvider(ICompositeControlRepository compositeControlRepository)
        {
            _compositeControlRepository = compositeControlRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> Compare(IEnumerable<CompositeControl> compositeControl)
        {
            var results = new List<ComparisonResult>();
            foreach (var control in compositeControl)
            {
                results.Add(await _compositeControlRepository.Compare(control));
            }
            return results;
        }

        public async Task ForceUpdate(IEnumerable<CompositeControl> compositeControls)
        {
            foreach (var control in compositeControls)
            {
                await _compositeControlRepository.ForceUpdateAsync(control.Id, control);
            }
        }

        public async Task<IEnumerable<CompositeControl>> GetByIds(IEnumerable<string> ids)
        {
            return await _compositeControlRepository.GetAllByIdsAsync(ids);
        }
    }
}
