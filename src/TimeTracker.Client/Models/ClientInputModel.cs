using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Client.Models
{
    public class ClientInputModel
    {
        [Required]
        public string Name { get; set; }
    }
}
