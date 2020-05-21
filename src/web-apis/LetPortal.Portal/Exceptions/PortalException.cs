using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions
{
    public class PortalException : CoreException
    {
        protected override string ExceptionService => "[Portal Exception]";

        public PortalException(ErrorCode errorCode)
            : base(errorCode)
        {
                           
        }
    }
}
