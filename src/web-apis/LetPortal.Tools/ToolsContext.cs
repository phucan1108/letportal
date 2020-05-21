using System;
using System.Collections.Generic;
using LetPortal.Core.Versions;

namespace LetPortal.Tools
{
    public class ToolsContext : IDisposable
    {
        public IEnumerable<IVersion> Versions { get; set; }

        public IVersionContext VersionContext { get; set; }

        public Core.Versions.Version LatestVersion { get; set; }

        public IVersionRepository VersionRepository { get; set; }

        public string VersionNumber { get; set; }

        public string PatchesFolder { get; set; }

        public bool AllowPatch { get; set; }

        public IServiceProvider Services { get; set; }

        public RootArgument Arguments { get; set; }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    VersionRepository.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }

    public class RootArgument
    {           
        public string Connection { get; set; }
                
        public string DatabseType { get; set; } = "mongodb";
        
        public string FilePath { get; set; }
        
        public string PatchesFolder { get; set; }
                
        public string Name { get; set; }
                
        public string UnpackMode { get; set; }
        
        public string App { get; set; }
        
        public string Command { get; set; }
                
        public string VersionNumber { get; set; }

        public string Output { get; set; }
    }
}
