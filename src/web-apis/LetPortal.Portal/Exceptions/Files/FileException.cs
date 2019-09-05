using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions.Files
{
    public class FileException : PortalException
    {
        protected override string ExceptionService => "[Media Exception]";

        public FileException(ErrorCode errorCode)
            : base(errorCode)
        {
        }
    }
}
