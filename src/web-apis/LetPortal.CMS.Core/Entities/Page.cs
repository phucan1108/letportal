using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Core.Entities
{
    [EntityCollection(Name = "pages")]
    public class Page : Entity
    {
        public string Name { get; set; }        

        /// <summary>
        /// If yes, this page has been moved or deleted
        /// Server must response HTTP 301
        /// </summary>
        public bool IsRedirected { get; set; }

        public string NextRedirectPage { get; set; }

        public bool Enable { get; set; }

        public GoogleMetadata GoogleMetadata { get; set; }

        public string PageTemplateId { get; set; }

        /// <summary>
        /// Each time user choose publish, this field will be used to display
        /// </summary>
        public string ChosenPageVersionId { get; set; }

        // Help to refer Site
        public string SiteId { get; set; }
    }
}
