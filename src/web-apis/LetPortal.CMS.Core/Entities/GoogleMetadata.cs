namespace LetPortal.CMS.Core.Entities
{
    public class GoogleMetadata
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Robots { get; set; }

        public bool AllowSiteLinkSearch { get; set; }

        public bool AllowGoogleTranslate { get; set; }

        /// <summary>
        /// Allow Google Assistant to read this page
        /// </summary>
        public bool AllowGoogleRead { get; set; }

        public bool IsAdultPage { get; set; }
    }
}
