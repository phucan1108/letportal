using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Https
{
    public class HttpResultModel
    {
        public bool IsSuccess { get; set; }

        public dynamic Result { get; set; }
    }
}
