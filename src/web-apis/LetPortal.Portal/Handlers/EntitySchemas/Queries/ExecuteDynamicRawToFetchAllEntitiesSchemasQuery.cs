namespace LetPortal.Portal.Handlers.EntitySchemas.Queries
{
    public class ExecuteDynamicRawToFetchAllEntitiesSchemasQuery
    {
        public string DatabaseId { get; set; }

        public string RawQuery { get; set; }

        public bool IsAggregation { get; set; }
    }
}
