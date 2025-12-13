using System.ComponentModel.DataAnnotations;

namespace LetPortal.Identity.Models
{
    public class LoginModel
    {
        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(30)]
        public string Password { get; set; }

        public string VersionInstalled { get; set; }

        public string SoftwareAgent { get; set; }

        public string ClientIp { get; set; }
    }
}
