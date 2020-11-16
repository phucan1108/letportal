using System.Collections;
using System.Collections.Generic;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Entities;

namespace LetPortal.CMS.Core.Models
{
    public class FlexiblePageModel
    {
        public GoogleMetadata GoogleMetadata { get; set; }

        public IList<SectionPartModel> Sections { get; set; }
    }

    public class SectionPartModel
    {
        public string TemplateKey { get; set; }

        public string Name { get; set; }

        public BindingType BindingType { get; set; }

        public string DatasourceName { get; set; }
    }
}
