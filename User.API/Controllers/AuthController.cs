using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using User.API.Contracts;
using User.API.ModelDtos;

namespace User.API.Controllers;

/// <summary>
/// Controller for handling authentication-related operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="authManager">The authentication manager.</param>
    public AuthController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registerDto">The user registration details.</param>
    /// <returns>A response indicating success or failure.</returns>
    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var errors = await _authManager.Register(registerDto);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { title = "An error occurred while processing request", status = 500, detail = ex.Message });
        }
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>A response containing the authentication token or an error message.</returns>
    [HttpPost]
    [Route("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Login([FromBody] AuthDto loginDto)
    {
        try
        {
            var authResponse = await _authManager.Login(loginDto);
            if (authResponse == null)
            {
                return Unauthorized();
            }
            return Ok(authResponse);
        }
        catch
        {
            return Unauthorized();
        }
    }
}