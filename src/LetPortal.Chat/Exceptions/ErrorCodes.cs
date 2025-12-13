using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Exceptions;

namespace LetPortal.Chat.Exceptions
{
    public class ErrorCodes
    {
        public static ErrorCode UserIsInCall = new ErrorCode
        {
            MessageCode = "VDSE0000001",
            MessageContent = "User is busy on call"
        };

        public static ErrorCode RequestUserIsInCall = new ErrorCode
        {
            MessageCode = "VDSE0000002",
            MessageContent = "You are on call in another device"
        };

        public static ErrorCode VideoCallHasBeenEnd = new ErrorCode
        {
            MessageCode = "VDSE000003",
            MessageContent = "Your call has been end"
        };

        public static ErrorCode WrongVideoRoom = new ErrorCode
        {
            MessageCode = "VDSE000004",
            MessageContent = "Wrong call"
        };
    }
}
