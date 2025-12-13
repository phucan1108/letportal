using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Exceptions;

namespace LetPortal.Microservices.Client.Exceptions
{
    public class MicroserviceClientException : CoreException
    {
        protected override string ExceptionService => "[Microservice Client Exception]";

        public MicroserviceClientException(ErrorCode errorCode)
            : base(errorCode)
        {

        }
    }
}
