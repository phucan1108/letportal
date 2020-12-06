namespace LetPortal.Microservices.Client
{
    public class ServiceOptions
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public string SaturnEndpoint { get; set; }

        public bool BypassSSL { get; set; }

        public int RetryCount { get; set; }

        public int DelayRetry { get; set; }
    }
}
