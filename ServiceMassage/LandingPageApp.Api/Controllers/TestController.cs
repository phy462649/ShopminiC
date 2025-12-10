using LandingPageApp.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LandingPageApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IAuthService? _authService;

        public TestController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return Ok($"Test controller works! AuthService: {(_authService != null ? "OK" : "NULL")}");
        }
    }
}
