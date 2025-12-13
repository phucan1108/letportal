using System.ComponentModel.DataAnnotations;

namespace LetPortal.Portal.Models.Apps
{
    public class PackageRequestModel
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public string Creator { get; set; }

        [Required]
        public string AppId { get; set; }
    }
}
