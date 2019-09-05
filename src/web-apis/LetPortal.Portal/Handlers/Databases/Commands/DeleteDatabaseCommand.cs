using System.ComponentModel.DataAnnotations;

namespace LetPortal.Portal.Handlers.Databases.Commands
{
    public class DeleteDatabaseCommand
    {
        [Required]
        public string Id { get; set; }
    }
}
