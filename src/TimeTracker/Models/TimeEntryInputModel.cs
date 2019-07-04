using System;
using TimeTracker.Domain;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single instance of time entry to add or modify.
    /// </summary>
    public class TimeEntryInputModel
    {
        /// <summary>
        /// Gets or sets the id of the user that this time entry belongs to.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the id of the project that this time entry refers to.
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the time entry date.
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the number of hours spent on a task.
        /// </summary>
        public int Hours { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Map the current model into an instance of <see cref="TimeEntry"/>.
        /// </summary>
        /// <param name="timeEntry">A <see cref="TimeEntry"/> instance to modify.</param>
        public void MapTo(TimeEntry timeEntry)
        {
            timeEntry.EntryDate = EntryDate;
            timeEntry.Hours = Hours;
            timeEntry.Description = Description;
        }
    }
}
