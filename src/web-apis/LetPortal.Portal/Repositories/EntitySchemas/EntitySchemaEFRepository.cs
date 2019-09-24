using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.EntitySchemas;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.EntitySchemas
{
    public class EntitySchemaEFRepository : EFGenericRepository<EntitySchema>, IEntitySchemaRepository
    {
        private readonly LetPortalDbContext _context;

        public EntitySchemaEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<EntitySchema> GetOneEntitySchemaAsync(string databaseId, string name)
        {
            var found = _context.EntitySchemas.First(a => a.DatabaseId == databaseId && a.Name == name);
            return Task.FromResult(found);
        }

        public Task UpsertEntitySchemasAsync(IEnumerable<EntitySchema> entitySchemas, bool isKeptSameName = false)
        {
            foreach(EntitySchema entitySchema in entitySchemas)
            {
                bool isExisted = _context.EntitySchemas.Any(a => a.Name == entitySchema.Name);
                if((isExisted && isKeptSameName) == false)
                {
                    entitySchema.Id = DataUtil.GenerateUniqueId();
                    _context.EntitySchemas.Add(entitySchema);
                }
            }

            _context.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
