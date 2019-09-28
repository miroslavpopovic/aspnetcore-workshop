using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeTracker.Data;
using TimeTracker.Models;

namespace TimeTracker.Controllers
{
    /// <summary>
    /// Users endpoint of TimeTracker API.
    /// </summary>
    [ApiController]
    [ApiVersion("2")]
    [Authorize]
    [Route("/api/v{version:apiVersion}/users")]
    public class UsersController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="UsersController"/> with given dependencies.
        /// </summary>
        /// <param name="dbContext">DB context instance.</param>
        /// <param name="logger">Logger instance.</param>
        public UsersController(TimeTrackerDbContext dbContext, ILogger<UsersController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Get a single user by id.
        /// </summary>
        /// <param name="id">Id of the user to retrieve.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserModel>> GetById(long id)
        {
            _logger.LogDebug($"Getting a user with id {id}");

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserModel.FromUser(user);
        }

        /// <summary>
        /// Get one page of users.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedList<UserModel>))]
        public async Task<ActionResult<PagedList<UserModel>>> GetPage(int page = 1, int size = 5)
        {
            _logger.LogDebug($"Getting a page {page} of users with page size {size}");

            var users = await _dbContext.Users
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedList<UserModel>
            {
                Items = users.Select(UserModel.FromUser),
                Page = page,
                PageSize = size,
                TotalCount = await _dbContext.Users.CountAsync()
            };
        }

        /// <summary>
        /// Delete a single user with the given id.
        /// </summary>
        /// <param name="id">Id of the user to delete.</param>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogDebug($"Deleting user with id {id}");

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Create a new user from the supplied data.
        /// </summary>
        /// <param name="model">Data to create the user from.</param>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserModel))]
        public async Task<ActionResult<UserModel>> Create(UserInputModel model)
        {
            _logger.LogDebug($"Creating a new user with name {model.Name}");

            var user = new Domain.User();
            model.MapTo(user);

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var resultModel = UserModel.FromUser(user);

            return CreatedAtAction(nameof(GetById), "users", new {id = user.Id, version = "2"}, resultModel);
        }

        /// <summary>
        /// Modify the user with the given id, using the supplied data.
        /// </summary>
        /// <param name="id">Id of the user to modify.</param>
        /// <param name="model">Data to modify the user from.</param>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserModel>> Update(long id, UserInputModel model)
        {
            _logger.LogDebug($"Updating user with id {id}");

            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            model.MapTo(user);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return UserModel.FromUser(user);
        }
    }
}
