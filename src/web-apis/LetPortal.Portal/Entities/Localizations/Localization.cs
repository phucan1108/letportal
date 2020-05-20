using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Pages;

namespace LetPortal.Portal.Entities.Localizations
{
    [EntityCollection(Name = LocalizationConstants.LocalizationCollection)]
    [Table(LocalizationConstants.LocalizationCollection)]
    public class Localization : Entity
    {
        public IList<LocalizationContent> LocalizationContents { get; set; }

        /// <summary>
        /// BCP 47
        /// Prefer to this list: https://docs.microsoft.com/en-us/openspecs/office_standards/ms-oe376/6c085406-a698-4e12-9d4d-c3b0ee3dbc4a
        /// </summary>
        [StringLength(20)]
        public string LocaleId { get; set; }
                        
        /// <summary>
        /// Reference to App Id
        /// </summary>
        [StringLength(50)]
        public string AppId { get; set; }
    }
}
