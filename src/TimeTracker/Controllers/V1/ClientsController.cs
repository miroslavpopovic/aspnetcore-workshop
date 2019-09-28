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
    /// Clients endpoint of TimeTracker API.
    /// </summary>
    [ApiController]
    [ApiVersion("1", Deprecated = true)]
    [Authorize]
    [Route("/api/v{version:apiVersion}/clients")]
    public class ClientsController : Controller
    {
        private readonly TimeTrackerDbContext _dbContext;
        private readonly ILogger<ClientsController> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="ClientsController"/> with given dependencies.
        /// </summary>
        /// <param name="dbContext">DB context instance.</param>
        /// <param name="logger">Logger instance.</param>
        public ClientsController(TimeTrackerDbContext dbContext, ILogger<ClientsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <summary>
        /// Get a single client by id.
        /// </summary>
        /// <param name="id">Id of the client to retrieve.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientModel>> GetById(long id)
        {
            _logger.LogDebug($"Getting a client with id {id}");

            var client = await _dbContext.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return ClientModel.FromClient(client);
        }

        /// <summary>
        /// Get one page of clients.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="size">Page size.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedList<ClientModel>))]
        public async Task<ActionResult<PagedList<ClientModel>>> GetPage(int page = 1, int size = 5)
        {
            _logger.LogDebug($"Getting a page {page} of clients with page size {size}");

            var clients = await _dbContext.Clients
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedList<ClientModel>
            {
                Items = clients.Select(ClientModel.FromClient),
                Page = page,
                PageSize = size,
                TotalCount = await _dbContext.Clients.CountAsync()
            };
        }

        /// <summary>
        /// Delete a single client with the given id.
        /// </summary>
        /// <param name="id">Id of the client to delete.</param>
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            _logger.LogDebug($"Deleting client with id {id}");

            var client = await _dbContext.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Create a new client from the supplied data.
        /// </summary>
        /// <param name="model">Data to create the client from.</param>
        [Authorize(Roles = "admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ClientModel))]
        public async Task<ActionResult<ClientModel>> Create(ClientInputModel model)
        {
            _logger.LogDebug($"Creating a new client with name {model.Name}");

            var client = new Client();
            model.MapTo(client);

            await _dbContext.Clients.AddAsync(client);
            await _dbContext.SaveChangesAsync();

            var resultModel = ClientModel.FromClient(client);

            return CreatedAtAction(nameof(GetById), "clients", new {id = client.Id, version = "1"}, resultModel);
        }

        /// <summary>
        /// Modify the client with the given id, using the supplied data.
        /// </summary>
        /// <param name="id">Id of the client to modify.</param>
        /// <param name="model">Data to modify the client from.</param>
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClientModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientModel>> Update(long id, ClientInputModel model)
        {
            _logger.LogDebug($"Updating client with id {id}");

            var client = await _dbContext.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            model.MapTo(client);

            _dbContext.Clients.Update(client);
            await _dbContext.SaveChangesAsync();

            return ClientModel.FromClient(client);
        }
    }
}
