using System.Collections.Generic;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Core.Entities
{
    [EntityCollection(Name = "sites")]
    public class Site : Entity
    {
        public string Name { get; set; }

        public List<string> Domains { get; set; }

        /// <summary>
        /// 'default' or 'en-Us'
        /// </summary>
        public string DefaultLocaleId { get; set; }

        public bool Enable { get; set; }

        public string DefaultPathWhenNotFound { get; set; }

        public SiteMap SiteMap { get; set; }

        public string ThemeId { get; set; }
    }
}
