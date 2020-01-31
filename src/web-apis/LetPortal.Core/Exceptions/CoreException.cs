using System;
using LetPortal.Core.Utils;

namespace LetPortal.Core.Exceptions
{
    public class CoreException : Exception
    {
        protected virtual string ExceptionService { get; set; }

        public ErrorCode ErrorCode { get; set; }

        public CoreException(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public override string ToString()
        {
            return $"{ExceptionService}: Code={ErrorCode.MessageCode} Content={ErrorCode.MessageContent}";
        }

        public string ToJsonString()
        {
            return ConvertUtil.SerializeObject(ErrorCode, true);
        }
    }
}
