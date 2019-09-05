using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Core.Security;
using System.Collections.Generic;

namespace LetPortal.Portal.Entities.Pages
{
    [EntityCollection(Name = "pages")]
    public class Page : BackupableEntity
    {      
        public string UrlPath { get; set; }

        public List<ShellOption> ShellOptions { get; set; }

        public List<PortalClaim> Claims { get; set; }

        public PageBuilder Builder { get; set; }

        public List<PageDatasource> PageDatasources { get; set; }

        public List<PageEvent> Events { get; set; }     

        public List<PageButton> Commands { get; set; }
    }

    #region Page Info

    public class ShellOption
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }
    }

    #endregion

    #region Page Builder
    public class PageBuilder
    {
        public List<PageSection> Sections { get; set; }
    }
    #endregion
}
