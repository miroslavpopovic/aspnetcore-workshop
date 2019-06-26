using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Domain
{
    public class TimeEntry
    {
        public long Id { get; set; }

        [Required]
        public User User { get; set; }

        [Required]
        public Project Project { get; set; }

        public DateTime EntryDate { get; set; }

        public int Hours { get; set; }

        public decimal HourRate { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
