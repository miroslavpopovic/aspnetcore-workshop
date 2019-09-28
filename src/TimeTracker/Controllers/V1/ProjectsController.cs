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
    /// Projects endpoint for TimeTracker API.
    /// </summary>
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [Authorize]
    [Route("/api/v{version:apiVersion}/projects")]
    public class ProjectsController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        private readonly ILogger<ProjectsController> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="ProjectsController"/> with given dependencies.
        /// </summary>
        /// <param name="dbContext">DB context instance.</param>
        /// <param name="logger">Logger instance.</param>
        public ProjectsController(TimeTrackerDbContext dbContext, ILogger<ProjectsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Get a single project by id.
        /// </summary>
        /// <param name="id">Id of the project to retrieve.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectModel>> GetById(long id)
        {
            _logger.LogDebug($"Getting a project with id {id}");

            var project = await _dbContext.Projects
                .Include(x => x.Client)
                .SingleOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return ProjectModel.FromProject(project);
        }

        /// <summary>
        /// Get one page of projects.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedList<ProjectModel>))]
        public async Task<ActionResult<PagedList<ProjectModel>>> GetPage(int page = 1, int size = 5)
        {
            _logger.LogDebug($"Getting a page {page} of projects with page size {size}");

            var projects = await _dbContext.Projects
                .Include(x => x.Client)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedList<ProjectModel>
            {
                Items = projects.Select(ProjectModel.FromProject),
                Page = page,
                PageSize = size,
                TotalCount = await _dbContext.Projects.CountAsync()
            };
        }

        /// <summary>
        /// Delete a single project with the given id.
        /// </summary>
        /// <param name="id">Id of the project to delete.</param>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogDebug($"Deleting project with id {id}");

            var project = await _dbContext.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Create a new project from the supplied data.
        /// </summary>
        /// <param name="model">Data to create the project from.</param>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProjectModel))]
        public async Task<ActionResult<ProjectModel>> Create(ProjectInputModel model)
        {
            _logger.LogDebug($"Creating a new project with name {model.Name}");

            var client = await _dbContext.Clients.FindAsync(model.ClientId);
            if (client == null)
            {
                return NotFound();
            }

            var project = new Project {Client = client};
            model.MapTo(project);

            await _dbContext.Projects.AddAsync(project);
            await _dbContext.SaveChangesAsync();

            var resultModel = ProjectModel.FromProject(project);

            return CreatedAtAction(nameof(GetById), "projects", new {id = project.Id, version = "1"}, resultModel);
        }

        /// <summary>
        /// Modify the project with the given id, using the supplied data.
        /// </summary>
        /// <param name="id">Id of the project to modify.</param>
        /// <param name="model">Data to modify the project from.</param>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProjectModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectModel>> Update(long id, ProjectInputModel model)
        {
            _logger.LogDebug($"Updating project with id {id}");

            var project = await _dbContext.Projects.FindAsync(id);
            var client = await _dbContext.Clients.FindAsync(model.ClientId);

            if (project == null || client == null)
            {
                return NotFound();
            }

            project.Client = client;
            model.MapTo(project);

            _dbContext.Projects.Update(project);
            await _dbContext.SaveChangesAsync();

            return ProjectModel.FromProject(project);
        }
    }
}
