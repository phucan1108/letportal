using LetPortal.Core.Persistences;

namespace LetPortal.ServiceManagement.Options
{
    public class CentralizedLogOptions
    {
        public DatabaseOptions Database { get; set; }

        public string EntityLogName { get; set; } = "servicelogs";
    }
}
