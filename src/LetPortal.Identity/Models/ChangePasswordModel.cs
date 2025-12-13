using System.ComponentModel.DataAnnotations;

namespace LetPortal.Identity.Models
{
    public class ChangePasswordModel
    {
        [Required]
        [MinLength(6)]
        [MaxLength(30)]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(30)]
        public string NewPassword { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(30)]
        public string ReNewPassword { get; set; }
    }
}
