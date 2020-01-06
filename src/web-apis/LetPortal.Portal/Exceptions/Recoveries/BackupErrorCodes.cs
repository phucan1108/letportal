using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions.Recoveries
{
    public class BackupErrorCodes
    {
        public static readonly ErrorCode ReachMaximumBackupObjects = new ErrorCode
        {
            MessageCode = "BSE000001",
            MessageContent = "Number of objects is greater than maximum"
        };
    }
}
