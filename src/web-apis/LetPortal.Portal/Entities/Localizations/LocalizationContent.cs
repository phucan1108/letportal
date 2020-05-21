using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Constants;

namespace LetPortal.Portal.Entities.Localizations
{
    [Table(LocalizationConstants.LocalizationContentTable)]
    public class LocalizationContent : Entity
    {
        /// <summary>
        /// Pattern: {section_name}.{control_name}.{options|validators}.{property_name}         
        /// userinfo.password.options.placeholder
        /// userinfo.password.options.label
        /// userinfo.password.validators.required
        /// </summary>
        [StringLength(100)]
        public string Key { get; set; }

        [StringLength(4000)]
        public string Text { get; set; }

        [StringLength(50)]
        public string LocalizationId { get; set; }

        public Localization Localization { get; set; }
    }
}
