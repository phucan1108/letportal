using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions.Databases
{
    public class DatabaseErrorCodes
    {
        public static readonly ErrorCode NotSupportedConnectionType = new ErrorCode
        {
            MessageCode = "DSE000001",
            MessageContent = "Not supported connection type."
        };

        public static readonly ErrorCode SQLNotSupportObjectType = new ErrorCode
        {
            MessageCode = "DSE000002",
            MessageContent = "SQL doesn't support object type to execute command."
        };
    }
}
