namespace LetPortal.Core.Exceptions
{
#pragma warning disable CS0659 // 'ErrorCode' overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class ErrorCode
#pragma warning restore CS0659 // 'ErrorCode' overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public string MessageCode { get; set; }

        public string MessageContent { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ErrorCode errorCode)
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
