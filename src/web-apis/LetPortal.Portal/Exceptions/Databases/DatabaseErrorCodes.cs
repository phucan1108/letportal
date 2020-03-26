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

        public static readonly ErrorCode LoopDataIsNotNull = new ErrorCode
        {
            MessageCode = "DSE000003",
            MessageContent = "A request loop data is null but the steps required, please check your configuration"
        };
    }
}
