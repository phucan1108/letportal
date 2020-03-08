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
}
