using LetPortal.Core.Persistences;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Core.Versions
{
    public class VersionEFRepository : EFGenericRepository<Version>, IVersionRepository
    {
        public VersionEFRepository(DbContext context) : base(context)
        {
        }
    }
}
