using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using User.API.Contracts;
using User.API.ModelDtos;

namespace User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly IAuthManager _authManager;

    public AuthController(IAuthManager authManager)
    {
        this._authManager = authManager;
    }
    
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
    
    [HttpPost]
    [Route("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Login([FromBody] AuthDto loginDto)
    {
        var res = await _authManager.Login(loginDto);
        if (res == null)
        {
            return Unauthorized();
        }
        return Ok(res);
    }
}