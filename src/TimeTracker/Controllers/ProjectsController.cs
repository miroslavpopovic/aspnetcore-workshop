using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeTracker.Data;
using TimeTracker.Domain;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    [ApiController]
    [Authorize]
    [Route("/api/projects")]
    public class ProjectsController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(TimeTrackerDbContext dbContext, ILogger<ProjectsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("{id}")]
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

        [HttpGet]
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

        [Authorize(Roles = "admin")]
        [HttpPost]
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

            return CreatedAtAction(nameof(GetById), "projects", new {id = project.Id}, resultModel);
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
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
