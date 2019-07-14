using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Client.Models
{
    public class UserInputModel
    {
        [Required]
        public string Name { get; set; }

        [Range(1, 1000)]
        public decimal HourRate { get; set; }
    }
}
