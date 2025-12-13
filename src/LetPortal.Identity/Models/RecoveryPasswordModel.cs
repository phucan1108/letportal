namespace LetPortal.Identity.Models
{
    public class RecoveryPasswordModel
    {
        public string ValidateCode { get; set; }

        public string UserId { get; set; }

        public string NewPassword { get; set; }
    }
}
