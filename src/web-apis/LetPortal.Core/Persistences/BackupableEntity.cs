using System;

namespace LetPortal.Core.Persistences
{
    public abstract class BackupableEntity : Entity
    {
        public string Name { get; set; }

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
