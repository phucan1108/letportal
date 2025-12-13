using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "roles")]
    [Table("roles")]
    public class Role : Entity
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string DisplayName { get; set; }

        [StringLength(250)]
        public string NormalizedName { get; set; }

        public List<BaseClaim> Claims { get; set; } = new List<BaseClaim>();
    }
}
