using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;

namespace LetPortal.CMS.Core.Repositories.Pages
{
    public class PageVersionMongoRepository : MongoGenericRepository<PageVersion>, IPageVersionRepository
    {
        public PageVersionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
