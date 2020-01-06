using LetPortal.Core.Files;
using LetPortal.Core.Persistences;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Options.Files
{
    public class FileOptions
    {                  
        public string DownloadableHost { get; set; }
        public FileStorageType FileStorageType { get; set; }

        public DiskStorageOptions DiskStorageOptions { get; set; }

        public DatabaseStorageOptions DatabaseStorageOptions { get; set; }

        public FileValidatorOptions ValidatorOptions { get; set; }
    }

    public class FileValidatorOptions
    { 
        public long MaximumFileSize { get; set; }

        public bool CheckFileExtension { get; set; }  

        public Dictionary<string, string> ExtensionMagicNumbers { get; set; }

        public string WhiteLists { get; set; }
    }

    public class DiskStorageOptions
    {  
        public bool IsStoredInTempFolder { get; set; }
              
        public bool AllowDayFolder { get; set; }

        public string Path { get; set; }
    }

    public class DatabaseStorageOptions
    {
        public bool SameAsPortal { get; set; }

        public DatabaseOptions DatabaseOptions { get; set; }
    }
}
