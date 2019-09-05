namespace LetPortal.Portal.Handlers.EntitySchemas.Commands
{
    public class FlushEntitySchemasInOneDatabaseCommand
    {
        public string DatabaseId { get; set; }

        public bool KeptSameName { get; set; }
    }
}
