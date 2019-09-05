using System.Collections.Generic;

namespace LetPortal.Core.Logger
{
    class LogEvenCache
    {
        public string TraceId { get; set; }

        public List<string> LogMessages { get; set; } = new List<string>();
    }
}
