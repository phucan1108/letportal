using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Core.Entities
{
    [EntityCollection(Name = "pageversions")]
    public class PageVersion : Entity
    {
        /// <summary>
        /// By default: 1, 2, 3
        /// User can add more short description such as 1 - New Year Page
        /// </summary>
        public string Name { get; set; }

        public string PageId { get; set; }

        public string Creator { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public List<PagePartManifest> Manifests { get; set; }
    }


    public class PagePartManifest
    {
        public string TemplateKey { get; set; }

        /// <summary>
        /// Affect only BindingType=Datasource
        /// Support many scenarios:
        /// queryparams - refer to query string parameters
        /// cookies - prefer to cookie value
        /// sessions - prefer to session value
        /// items - prefer to HttpContext.Items
        /// resolvers - prefer to data from resovlers
        /// </summary>
        public string DatasourceKey { get; set; }

        public BindingType BindingType { get; set; }

        public List<VersionValueList> ValuesList { get; set; }
    }
}
