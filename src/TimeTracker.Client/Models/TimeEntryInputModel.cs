using System;
using System.ComponentModel.DataAnnotations;

namespace TimeTracker.Client.Models
{
    public class TimeEntryInputModel
    {
        public long UserId { get; set; }

        [Required]
        public long ProjectId { get; set; }

        public DateTime EntryDate { get; set; }

        [Range(1, 24)]
        public int Hours { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
