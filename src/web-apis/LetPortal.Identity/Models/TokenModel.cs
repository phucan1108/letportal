namespace LetPortal.Identity.Models
{
    public class TokenModel
    {
        public string Token { get; set; }

        public long Exp { get; set; }

        public string RefreshToken { get; set; }

        public long ExpRefresh { get; set; }

        public string UserSessionId { get; set; }
    }
}
