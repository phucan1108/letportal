namespace LetPortal.Microservices.Server.Options
{
    public class ServiceConfigurationOptions
    {
        public string BasedFolder { get; set; } = "Files";

        public string SharedFolder { get; set; } = "Shared";

        public string IgnoreCombinedServices { get; set; } = "Portal";
    }
}
