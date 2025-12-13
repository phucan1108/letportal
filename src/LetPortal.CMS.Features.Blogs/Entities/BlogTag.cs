using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Features.Blogs.Entities
{
    [EntityCollection(Name = "blogtags")]
    public class BlogTag : Entity
    {
        public string Tag { get; set; }
    }
}
