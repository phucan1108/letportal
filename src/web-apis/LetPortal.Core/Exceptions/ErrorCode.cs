namespace LetPortal.Core.Exceptions
{
    public class ErrorCode
    {
        public string MessageCode { get; set; }

        public string MessageContent { get; set; }

        public override bool Equals(object obj)
        {
            if(obj is ErrorCode errorCode)
            {
                return errorCode.MessageCode == MessageCode && errorCode.MessageContent == MessageContent;
            }
            else
            {
                return false;
            }
        }
    }
}
