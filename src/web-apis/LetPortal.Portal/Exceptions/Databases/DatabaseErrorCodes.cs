using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions.Databases
{
    public class DatabaseErrorCodes
    {
        public static readonly ErrorCode NotSupportedConnectionType = new ErrorCode
        {
            MessageCode = "DSE000001",
            MessageContent = "Not supported connection type"
        };
    }
}
