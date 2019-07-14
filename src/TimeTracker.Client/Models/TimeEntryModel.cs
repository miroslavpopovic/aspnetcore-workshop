using System;

namespace TimeTracker.Client.Models
{
    public class TimeEntryModel
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime EntryDate { get; set; }
        public int Hours { get; set; }
        public decimal HourRate { get; set; }
        public string Description { get; set; }
    }
}
