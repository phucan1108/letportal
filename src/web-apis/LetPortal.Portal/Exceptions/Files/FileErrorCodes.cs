using LetPortal.Core.Exceptions;

namespace LetPortal.Portal.Exceptions.Files
{
    public class FileErrorCodes
    {
        public static readonly ErrorCode WrongFileExtension = new ErrorCode
        {
            MessageCode = "MSE000001",
            MessageContent = "A uploaded file is wrong format and extension"
        };

        public static readonly ErrorCode ReachMaximumFile = new ErrorCode
        {
            MessageCode = "MSE000002",
            MessageContent = "A uploaded file is reached maximum size"
        };
    }
}
