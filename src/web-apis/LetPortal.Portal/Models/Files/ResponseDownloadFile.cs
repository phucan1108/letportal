using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Files
{
    public class ResponseDownloadFile
    {
        public string FileName { get; set; }

        public string MIMEType { get; set; }

        public byte[] FileBytes { get; set; }
    }
}
