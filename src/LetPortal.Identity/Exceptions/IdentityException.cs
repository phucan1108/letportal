using LetPortal.Core.Exceptions;

namespace LetPortal.Identity.Exceptions
{
    public class IdentityException : CoreException
    {
        protected override string ExceptionService => "[Identity-Service]";

        public IdentityException(ErrorCode errorCode)
            : base(errorCode)
        {
        }
    }
}
