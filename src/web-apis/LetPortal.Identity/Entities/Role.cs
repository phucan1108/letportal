using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "roles")]
    [Table("roles")]
    public class Role : Entity
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string NormalizedName { get; set; }

        public List<BaseClaim> Claims { get; set; } = new List<BaseClaim>();
    }
}
