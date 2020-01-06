using LetPortal.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Exceptions.Recoveries
{
    public class BackupException : PortalException
    {
        protected override string ExceptionService => "[Backup Exception]";

        public BackupException(ErrorCode errorCode)
            : base(errorCode)
        {
        }
    }
}
