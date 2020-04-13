using System.ComponentModel.DataAnnotations;

namespace LetPortal.Core.Persistences
{
    public abstract class Entity
    {
        [StringLength(50)]
        public string Id { get; set; }

        public virtual void Check()
        {

        }
    }
}
