using System.ComponentModel.DataAnnotations;

namespace LetPortal.Core.Persistences
{
    public abstract class Entity
    {
        public string Id { get; set; }

        public virtual void Check()
        {

        }
    }
}
