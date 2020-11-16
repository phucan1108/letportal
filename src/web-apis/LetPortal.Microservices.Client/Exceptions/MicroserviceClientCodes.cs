using LetPortal.Core.Exceptions;

namespace LetPortal.Microservices.Client.Exceptions
{
    public class MicroserviceClientCodes
    {
        public static readonly ErrorCode FailedRegisterFromServer = new ErrorCode
        {
            MessageCode = "MCSE000001",
            MessageContent = "Server respond a failed message when registering"
        };
    }
}
