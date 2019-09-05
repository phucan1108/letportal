using LetPortal.Portal.Entities.EntitySchemas;
using System.ComponentModel.DataAnnotations;

namespace LetPortal.Portal.Handlers.EntitySchemas.Commands
{
    public class UpdateEntitySchemaCommand
    {
        [Required]
        public EntitySchema EntitySchema { get; set; }
    }
}
