using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.CMS.Core.Annotations
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class EditableManifestAttribute : Attribute
    {
        public bool Enable { get; set; }
    }
}
