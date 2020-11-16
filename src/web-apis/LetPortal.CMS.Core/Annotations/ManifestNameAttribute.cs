using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.CMS.Core.Annotations
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class ManifestNameAttribute : Attribute
    {
        private string Name;

        public ManifestNameAttribute(string name)
        {
            Name = name;
        }

        public string GetName()
        {
            return Name;
        }
    }
}
