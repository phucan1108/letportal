using System;
using LetPortal.CMS.Core.Abstractions;

namespace LetPortal.CMS.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ThemePartAttribute : Attribute
    {
        public string Name { get; set; }

        public BindingType BindingType { get; set; }

        public Type Type { get; set; }
    }
}
