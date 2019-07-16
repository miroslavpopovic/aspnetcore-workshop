using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Client.Models
{
    public class ProjectInputModel
    {
        [Required]
        public string Name { get; set; }

        public long ClientId { get; set; }
    }
}
