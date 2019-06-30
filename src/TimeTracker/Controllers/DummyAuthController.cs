using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TimeTracker.Controllers
{
    // DEMO PURPOSE ONLY!
    public class DummyAuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public DummyAuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // NOT FOR PRODUCTION USE!!!
        // you will need a robust auth implementation for production
        // i.e. try IdentityServer4
        [Route("/get-token")]
        public IActionResult GenerateToken(string name = "aspnetcore-workshop-demo", bool admin = false)
        {
            var jwt = JwtTokenGenerator
                .Generate(name, admin, _configuration["Tokens:Issuer"], _configuration["Tokens:Key"]);

            return Ok(jwt);
        }
    }
}
