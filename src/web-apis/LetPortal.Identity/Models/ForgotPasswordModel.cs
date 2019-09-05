using System.ComponentModel.DataAnnotations;

namespace LetPortal.Identity.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
