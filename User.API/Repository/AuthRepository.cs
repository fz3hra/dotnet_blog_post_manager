using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using User.API.Configurations;
using User.API.Contracts;
using User.API.Data;
using User.API.ModelDtos;

namespace User.API.Repository;

/// <summary>
/// Repository for handling authentication operations.
/// </summary>
public class AuthRepository : IAuthManager
{
    private readonly UserManager<UserApi> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public AuthRepository(
        UserManager<UserApi> userManager,
        IMapper mapper,
        IConfiguration configuration)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    
    // Include XML comments for all methods in AuthRepository
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registerDto">The registration details.</param>
    /// <returns>A list of identity errors, if any.</returns>
    public async Task<IEnumerable<IdentityError>> Register(RegisterDto registerDto)
    {
        try
        {
            if (registerDto == null)
            {
                throw new ArgumentNullException(nameof(registerDto));
            }

            if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password))
            {
                return new List<IdentityError>
                {
                    new() { Code = "InvalidInput", Description = "Email and password are required" }
                };
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return new List<IdentityError>
                {
                    new() { Code = "DuplicateEmail", Description = "Email is already registered" }
                };
            }

            var user = _mapper.Map<UserApi>(registerDto);
            user.Email = registerDto.Email;
            user.FirstName = registerDto.FirstName;
            user.LastName = registerDto.LastName;
            user.UserName = registerDto.UserName;

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (!roleResult.Succeeded)
                {
                    return roleResult.Errors;
                }
            }

            return result.Errors;
        }
        catch (Exception ex)
        {
            return new List<IdentityError>
            {
                new() { Code = "RegistrationError", Description = $"Registration failed: {ex.Message}" }
            };
        }
    }

    /// <summary>
    /// Logs in a user and generates a JWT token.
    /// </summary>
    /// <param name="loginDto">The login details.</param>
    /// <returns>The authentication response with a token.</returns>
    public async Task<AuthResponseDto> Login(AuthDto loginDto)
    {
        try
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }

            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                throw new ArgumentException("Email and password are required");
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new InvalidOperationException("No account exists with this email address");

            }

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return null;
            }

            var token = await GenerateToken(user);
            return new AuthResponseDto
            {
                UserId = user.Id,
                Token = token,
                UserName = user.UserName,
                Email = user.Email
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<string> GenerateToken(UserApi user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var jwtKey = _configuration["JwtSettings:Key"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var durationStr = _configuration["JwtSettings:DurationInMinutes"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(issuer) ||
                string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(durationStr))
            {
                throw new InvalidOperationException("JWT settings are not properly configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Sub, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
            }
            .Union(userClaims)
            .Union(roleClaims);

            if (!int.TryParse(durationStr, out int durationInMinutes))
            {
                durationInMinutes = 60;
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Token generation failed", ex);
        }
    }

    /// <summary>
    /// Refreshes a token (not implemented).
    /// </summary>
    /// <param name="refreshToken">The refresh token.</param>
    /// <returns>A refreshed authentication token.</returns>
    public Task<AuthResponseDto> RefreshToken(string refreshToken)
    {
        throw new NotImplementedException("Refresh token functionality is not implemented");
    }
}