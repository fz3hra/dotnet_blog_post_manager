using Microsoft.AspNetCore.Identity;
using User.API.Data;
using User.API.ModelDtos;

namespace User.API.Contracts;

public interface IAuthManager
{
    Task<IEnumerable<IdentityError>> Register(RegisterDto registerDto);
    Task<AuthResponseDto> Login(AuthDto loginDto);
    Task<AuthResponseDto> RefreshToken(string refreshToken);
}