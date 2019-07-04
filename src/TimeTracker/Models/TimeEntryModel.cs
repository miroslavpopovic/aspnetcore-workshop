using System;
using TimeTracker.Domain;

namespace TimeTracker.Models
{
    /// <summary>
    /// Represents a single time entry.
    /// </summary>
    public class TimeEntryModel
    {
        /// <summary>
        /// Gets or sets the id of the time entry.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the user that this time entry belongs to.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user that this time entry belongs to.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the id of the project that this time entry refers to.
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the name of the project that this time entry refers to.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the id of the client that this time entry refers to.
        /// </summary>
        public long ClientId { get; set; }

        /// <summary>
        /// Gets or sets the name of the client that this time entry refers to.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the time entry date.
        /// </summary>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the number of hours spent on the task.
        /// </summary>
        public int Hours { get; set; }

        /// <summary>
        /// Gets or sets the user's hour rate for this time entry.
        /// </summary>
        public decimal HourRate { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="TimeEntryModel"/> from the instance of <see cref="TimeEntry"/>.
        /// </summary>
        /// <param name="timeEntry">A <see cref="TimeEntry"/> instance to convert to <see cref="TimeEntryModel"/>.</param>
        /// <returns>A new instance of <see cref="TimeEntryModel"/>.</returns>
        public static TimeEntryModel FromTimeEntry(TimeEntry timeEntry)
        {
            return new TimeEntryModel
            {
                Id = timeEntry.Id,
                UserId = timeEntry.User.Id,
                UserName = timeEntry.User.Name,
                ProjectId = timeEntry.Project.Id,
                ProjectName = timeEntry.Project.Name,
                ClientId = timeEntry.Project.Client.Id,
                ClientName = timeEntry.Project.Client.Name,
                EntryDate = timeEntry.EntryDate,
                Hours = timeEntry.Hours,
                HourRate = timeEntry.HourRate,
                Description = timeEntry.Description
            };
        }
    }
}
