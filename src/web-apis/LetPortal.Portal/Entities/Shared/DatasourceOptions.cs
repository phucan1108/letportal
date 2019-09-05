namespace LetPortal.Portal.Entities.Shared
{
    public class DatasourceOptions
    {
        public DatasourceControlType Type { get; set; }

        public DatasourceStaticOptions DatasourceStaticOptions { get; set; }

        public DatabaseOptions DatabaseOptions { get; set; }

        public HttpServiceOptions HttpServiceOptions { get; set; }

        public string TriggeredEvents { get; set; }
    }

    public class DynamicListDatasourceOptions : DatasourceOptions
    {
        public string OutputMapProjection { get; set; }
    }

    public enum DatasourceControlType
    {
        StaticResource,
        Database,
        WebService
    }
}
