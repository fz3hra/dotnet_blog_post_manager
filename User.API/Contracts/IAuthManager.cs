using Microsoft.AspNetCore.Identity;
using User.API.Data;
using User.API.ModelDtos;

namespace User.API.Contracts;

/// <summary>
/// Interface for authentication management.
/// </summary>
public interface IAuthManager
{
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registerDto">The user registration details.</param>
    /// <returns>A list of identity errors, if any.</returns>
    Task<IEnumerable<IdentityError>> Register(RegisterDto registerDto);

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="loginDto">The login credentials.</param>
    /// <returns>An authentication response containing the user token.</returns>
    Task<AuthResponseDto> Login(AuthDto loginDto);

    /// <summary>
    /// Refreshes an expired authentication token.
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>An authentication response containing a new token.</returns>
    Task<AuthResponseDto> RefreshToken(string refreshToken);
}