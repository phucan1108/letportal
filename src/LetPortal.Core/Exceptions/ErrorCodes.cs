namespace LetPortal.Core.Exceptions
{
    public class ErrorCodes
    {
        public static readonly ErrorCode InternalException = new ErrorCode
        {
            MessageCode = "CSE000500",
            MessageContent = "Oops, there are working something went wrong"
        };

        public static readonly ErrorCode NameAlreadyExistException = new ErrorCode
        {
            MessageCode = "CSE000001",
            MessageContent = "A name is already used by another"
        };
    }
}
