using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Components.Controls;
using LetPortal.Portal.Models.Shared;

namespace LetPortal.Portal.Repositories.Components
{
    public interface ICompositeControlRepository : IGenericRepository<CompositeControl>
    {
        Task<IEnumerable<ShortEntityModel>> GetShortEntities(string keyword);
    }
}
