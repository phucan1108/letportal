using LetPortal.Core.Exceptions;

namespace LetPortal.CMS.Core.Exceptions
{
    public class CMSException : CoreException
    {
        protected override string ExceptionService => "LetPortalCMS";

        public CMSException(ErrorCode errorCode)
            : base(errorCode)
        {
        }
    }
}
