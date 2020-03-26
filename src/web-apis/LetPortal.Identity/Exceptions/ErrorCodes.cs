using LetPortal.Core.Exceptions;

namespace LetPortal.Identity.Exceptions
{
    public class ErrorCodes
    {
        public static readonly ErrorCode CannotSignIn = new ErrorCode
        {
            MessageCode = "IDSE000001",
            MessageContent = "Invalid username or password"
        };

        public static readonly ErrorCode CannotDeactiveRefreshToken = new ErrorCode
        {
            MessageCode = "IDSE000002",
            MessageContent = "Cannot deactive the refresh token"
        };

        public static readonly ErrorCode UsernameHasBeenRegistered = new ErrorCode
        {
            MessageCode = "IDES000003",
            MessageContent = "Username has been registered"
        };

        public static readonly ErrorCode WrongUserName = new ErrorCode
        {
            MessageCode = "IDES000004",
            MessageContent = "Invalid Username"
        };

        public static readonly ErrorCode PasswordDoesNotMatchWithRePassword = new ErrorCode
        {
            MessageCode = "IDES000005",
            MessageContent = "Password doesn't match with re-password"
        };

        public static readonly ErrorCode CannotChangePassword = new ErrorCode
        {
            MessageCode = "IDES000006",
            MessageContent = "A password can't be changed, please try again"
        };

        public static readonly ErrorCode CannotUpdateClaimsOfUser = new ErrorCode
        {
            MessageCode = "IDES000007",
            MessageContent = "User can't update information, please try again"
        };
    }
}
