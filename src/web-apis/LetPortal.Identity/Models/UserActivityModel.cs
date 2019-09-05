using LetPortal.Identity.Entities;
using System.ComponentModel.DataAnnotations;

namespace LetPortal.Identity.Models
{
    public class UserActivityModel
    {
        [Required]
        public string UserSessionId { get; set; }

        [Required]
        public string TraceId { get; set; }

        [Required]
        public string ActivityName { get; set; }

        [Required]
        public string Content { get; set; }

        public ActivityType ActivityType { get; set; }
    }
}
