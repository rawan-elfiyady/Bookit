using System;
using System.Linq;
using System.Threading.Tasks;
using Bookit.Models;
using Bookit.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bookit.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private static readonly string[] AllowedRoles = { "Admin", "User", "Librarian" };

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Validation failed.", errors = ModelState });
            }

            // Ensure Role is provided and valid
            if (string.IsNullOrWhiteSpace(request.Role) || !AllowedRoles.Contains(request.Role, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Invalid role. Allowed values: Admin, User, Librarian." });
            }

            if (string.Equals(request.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "Admins cannot register themselves." });
            }

            var result = await _authService.RegisterUser(request);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { token = result.Token, Id = result.Id, role = result.Role });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Validation failed.", errors = ModelState });
            }

            var result = await _authService.LoginUser(request);
            if (!result.Success)
            {
                return Unauthorized(new { message = result.Message });
            }

            return Ok(new { token = result.Token, Id = result.Id, role = result.Role });
        }
    }
}
