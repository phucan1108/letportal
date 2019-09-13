using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions.Databases
{
    public class DatabaseException : PortalException
    {
        protected override string ExceptionService => "[Database Exception]";

        public DatabaseException(ErrorCode errorCode)
            : base(errorCode)
        {
        }
    }
}
