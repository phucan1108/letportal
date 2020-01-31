using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions.Recoveries
{
    public class BackupException : PortalException
    {
        protected override string ExceptionService => "[Backup Exception]";

        public BackupException(ErrorCode errorCode)
            : base(errorCode)
        {
        }
    }
}
