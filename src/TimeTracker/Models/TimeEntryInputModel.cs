using System;

namespace TimeTracker.Models
{
    public class TimeEntryInputModel
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public DateTime EntryDate { get; set; }
        public int Hours { get; set; }
        public decimal HourRate { get; set; }
        public string Description { get; set; }
    }
}
