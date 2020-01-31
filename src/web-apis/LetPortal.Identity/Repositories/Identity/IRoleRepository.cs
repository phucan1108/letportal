﻿using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Identity.Repositories.Identity
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        Task<Role> GetByNameAsync(string name);

        Task<Dictionary<string, List<BaseClaim>>> GetBaseClaims(string[] roles);
    }
}
