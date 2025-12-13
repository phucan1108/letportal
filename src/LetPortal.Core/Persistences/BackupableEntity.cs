using System;
using System.ComponentModel.DataAnnotations;

namespace LetPortal.Core.Persistences
{
    public abstract class BackupableEntity : Entity
    {
        [StringLength(250)]
        public string Name { get; set; }

        [StringLength(250)]
        public string DisplayName { get; set; }

        public long TimeSpan { get; set; }

        public override void Check()
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                DisplayName = Name;
            }
            TimeSpan = DateTime.UtcNow.Ticks;
        }
    }
}
