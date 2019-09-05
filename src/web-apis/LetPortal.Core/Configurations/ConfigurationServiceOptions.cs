namespace LetPortal.Core.Configurations
{
    public class ConfigurationServiceOptions
    {
        public string Endpoint { get; set; }

        public int RetryCount { get; set; }

        public int DelayRetry { get; set; }
    }
}
