namespace LetPortal.Core
{
    public class LetPortalMiddlewareOptions
    {
        /// <summary>
        /// Ensure UserSessionId must be provided in Header
        /// Default: false
        /// </summary>
        public bool EnableCheckUserSession { get; set; }

        /// <summary>
        /// Ensure TraceId must be provided in Header
        /// Default: false
        /// </summary>
        public bool EnableCheckTraceId { get; set; }

        /// <summary>
        /// Allow to push log to Service Management, however, it requires EnableCheckUserSession is true
        /// Default: false
        /// </summary>
        public bool EnableWrapException { get; set; }

        /// <summary>
        /// Allow to skip check UserSessionId and TraceId in specific urls
        /// </summary>
        public string[] SkipCheckUrls { get; set; }
    }
}
