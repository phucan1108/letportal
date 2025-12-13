using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Portal.Repositories.EntitySchemas
{
    public class EntitySchemaEFRepository : EFGenericRepository<EntitySchema>, IEntitySchemaRepository
    {
        private readonly PortalDbContext _context;

        public EntitySchemaEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<EntitySchema> GetOneEntitySchemaAsync(string databaseId, string name)
        {
            var found = _context.EntitySchemas.First(a => a.DatabaseId == databaseId && a.Name == name);
            return Task.FromResult(found);
        }

        public Task UpsertEntitySchemasAsync(
            IEnumerable<EntitySchema> entitySchemas,
            string databaseId,
            bool isKeptSameName = false)
        {
            foreach (var entitySchema in entitySchemas)
            {
                var isExisted = false;

                // Hotfix for MySQL issue: https://bugs.mysql.com/bug.php?id=92987
                if (_context.ConnectionType == ConnectionType.MySQL)
                {
                    var exist = _context.EntitySchemas.FirstOrDefault(a => a.Name == entitySchema.Name && a.DatabaseId == databaseId);
                    isExisted = exist != null;
                }
                else
                {
                    isExisted = _context.EntitySchemas.Any(a => a.Name == entitySchema.Name && a.DatabaseId == databaseId);
                }

                if ((isExisted && isKeptSameName) == false)
                {
                    entitySchema.Id = DataUtil.GenerateUniqueId();
                    entitySchema.DatabaseId = databaseId;
                    _context.EntitySchemas.Add(entitySchema);
                }
            }

            _context.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
