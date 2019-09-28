using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeTracker.Data;
using TimeTracker.Domain;
using TimeTracker.Models;

namespace TimeTracker.Controllers.V1
{
    /// <summary>
    /// Time entries endpoint of TimeTracker API.
    /// </summary>
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [Authorize]
    [Route("/api/v{version:apiVersion}/time-entries")]
    public class TimeEntriesController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        private readonly ILogger<TimeEntriesController> _logger;

        /// <summary>
        /// Creates a new instance of see <see cref="TimeEntriesController"/> with given dependencies.
        /// </summary>
        /// <param name="dbContext">DB context instance.</param>
        /// <param name="logger">Logger instance.</param>
        public TimeEntriesController(TimeTrackerDbContext dbContext, ILogger<TimeEntriesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Get a single time entry by id.
        /// </summary>
        /// <param name="id">Id of the time entry to retrieve.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeEntryModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TimeEntryModel>> GetById(long id)
        {
            _logger.LogDebug($"Getting a time entry with id {id}");

            var timeEntry = await _dbContext.TimeEntries
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Project.Client)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (timeEntry == null)
            {
                return NotFound();
            }

            return TimeEntryModel.FromTimeEntry(timeEntry);
        }

        /// <summary>
        /// Gets a list of time entries for a specified user and month.
        /// </summary>
        /// <param name="userId">User id to get the entries for.</param>
        /// <param name="year">Year of the time entry.</param>
        /// <param name="month">Month of the time entry.</param>
        [HttpGet("user/{userId}/{year}/{month}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeEntryModel[]))]
        public async Task<ActionResult<TimeEntryModel[]>> GetByUserAndMonth(long userId, int year, int month)
        {
            _logger.LogDebug($"Getting all time entries for month {year}-{month} for user with id {userId}");

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var timeEntries = await _dbContext.TimeEntries
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Project.Client)
                .Where(x => x.User.Id == userId && x.EntryDate >= startDate && x.EntryDate < endDate)
                .OrderBy(x => x.EntryDate)
                .ToListAsync();

            return timeEntries
                .Select(TimeEntryModel.FromTimeEntry)
                .ToArray();
        }

        /// <summary>
        /// Get one page of time entries.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedList<TimeEntryModel>))]
        public async Task<ActionResult<PagedList<TimeEntryModel>>> GetPage(int page = 1, int size = 5)
        {
            _logger.LogDebug($"Getting a page {page} of time entries with page size {size}");

            var timeEntries = await _dbContext.TimeEntries
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Project.Client)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedList<TimeEntryModel>
            {
                Items = timeEntries.Select(TimeEntryModel.FromTimeEntry),
                Page = page,
                PageSize = size,
                TotalCount = await _dbContext.TimeEntries.CountAsync()
            };
        }

        /// <summary>
        /// Delete a single time entry with the given id.
        /// </summary>
        /// <param name="id">Id of the time entry to delete.</param>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogDebug($"Deleting time entries with id {id}");

            var timeEntry = await _dbContext.TimeEntries.FindAsync(id);

            if (timeEntry == null)
            {
                return NotFound();
            }

            _dbContext.TimeEntries.Remove(timeEntry);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Create a new time entry from the supplied data.
        /// </summary>
        /// <param name="model">Data to create the time entry from.</param>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TimeEntryModel))]
        public async Task<ActionResult<TimeEntryModel>> Create(TimeEntryInputModel model)
        {
            _logger.LogDebug(
                $"Creating a new time entry for user {model.UserId}, project {model.ProjectId} and date {model.EntryDate}");

            var user = await _dbContext.Users.FindAsync(model.UserId);
            var project = await _dbContext.Projects
                .Include(x => x.Client) // Necessary for mapping to TimeEntryModel later
                .SingleOrDefaultAsync(x => x.Id == model.ProjectId);

            if (user == null || project == null)
            {
                return NotFound();
            }

            var timeEntry = new TimeEntry {User = user, Project = project, HourRate = user.HourRate};
            model.MapTo(timeEntry);

            await _dbContext.TimeEntries.AddAsync(timeEntry);
            await _dbContext.SaveChangesAsync();

            var resultModel = TimeEntryModel.FromTimeEntry(timeEntry);

            return CreatedAtAction(nameof(GetById), "TimeEntries", new {id = timeEntry.Id, version = "1"}, resultModel);
        }

        /// <summary>
        /// Modify the time entry with the given id, using the supplied data.
        /// </summary>
        /// <param name="id">Id of the time entry to modify.</param>
        /// <param name="model">Data to modify the time entry from.</param>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TimeEntryModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TimeEntryModel>> Update(long id, TimeEntryInputModel model)
        {
            _logger.LogDebug($"Updating time entry with id {id}");

            var timeEntry = await _dbContext.TimeEntries
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Project.Client)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (timeEntry == null)
            {
                return NotFound();
            }

            model.MapTo(timeEntry);

            _dbContext.TimeEntries.Update(timeEntry);
            await _dbContext.SaveChangesAsync();

            return TimeEntryModel.FromTimeEntry(timeEntry);
        }
    }
}
