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
    }
}
