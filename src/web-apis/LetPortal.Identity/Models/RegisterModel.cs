using System.ComponentModel.DataAnnotations;

namespace LetPortal.Identity.Models
{
    public class RegisterModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Password { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Repassword { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
