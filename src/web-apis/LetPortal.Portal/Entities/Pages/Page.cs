using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Core.Security;
using LetPortal.Portal.Entities.Localizations;

namespace LetPortal.Portal.Entities.Pages
{
    [EntityCollection(Name = "pages")]
    [Table("pages")]
    public class Page : BackupableEntity
    {
        [StringLength(250)]
        public string UrlPath { get; set; }

        [StringLength(50)]
        public string AppId { get; set; }

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
