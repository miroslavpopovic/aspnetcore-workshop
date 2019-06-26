using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Domain
{
    public class User
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public decimal HourRate { get; set; }
    }
}
