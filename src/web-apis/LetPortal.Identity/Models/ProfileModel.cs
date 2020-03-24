using System.ComponentModel.DataAnnotations;

namespace LetPortal.Identity.Models
{
    public class ProfileModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(250)]
        public string FullName { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(250)]
        [Url]
        public string Avatar { get; set; }
    }
}
