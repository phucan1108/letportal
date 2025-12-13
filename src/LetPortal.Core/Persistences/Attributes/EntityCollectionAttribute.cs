using System;

namespace LetPortal.Core.Persistences.Attributes
{
    public class EntityCollectionAttribute : Attribute
    {
        public string Name { get; set; }

        public bool IsUniqueBackup { get; set; } = true;
    }
}
