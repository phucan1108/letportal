using System.ComponentModel.DataAnnotations;

namespace LetPortal.Portal.Handlers.Databases.Commands
{
    public class CreateDatabaseCommand
    {
        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(255)]
        public string ConnectionString { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string DataSource { get; set; }

        [Required]
        public string DatabaseConnectionType { get; set; }
    }
}
