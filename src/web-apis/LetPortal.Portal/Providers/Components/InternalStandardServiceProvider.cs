using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Repositories.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Components
{
    public class InternalStandardServiceProvider : IStandardServiceProvider
    {
        private readonly IStandardRepository _standardRepository;

        public InternalStandardServiceProvider(IStandardRepository standardRepository)
        {
            _standardRepository = standardRepository;
        }

        public async Task<IEnumerable<ComparisonResult>> CompareStandardComponent(IEnumerable<StandardComponent> standardComponents)
        {
            var results = new List<ComparisonResult>();
            foreach(var standard in standardComponents)
            {
                results.Add(await _standardRepository.Compare(standard));
            }
            return results;
        }

        public async Task ForceUpdateStandards(IEnumerable<StandardComponent> standards)
        {
            foreach(var standard in standards)
            {
                await _standardRepository.ForceUpdateAsync(standard.Id, standard);
            }
        }

        public async Task<IEnumerable<StandardComponent>> GetStandardComponentsByIds(IEnumerable<string> ids)
        {
            return await _standardRepository.GetAllByIdsAsync(ids);
        }
    }
}
