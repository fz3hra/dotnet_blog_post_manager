using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using User.API.Configurations;
using User.API.Contracts;
using User.API.Data;
using User.API.ModelDtos;

namespace User.API.Repository;

public class AuthRepository: IAuthManager
{
    private readonly UserManager<UserApi> _userManager;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    
    public AuthRepository(UserManager<UserApi> userManager, IMapper mapper, IConfiguration configuration)
    {
        this._userManager = userManager;
        this._mapper = mapper;
        this._configuration = configuration;
    }
    public async Task<IEnumerable<IdentityError>> Register(RegisterDto registerDto)
    {
        var user = _mapper.Map<UserApi>(registerDto);
        user.Email = registerDto.Email;
        user.FirstName = registerDto.FirstName;
        user.LastName = registerDto.LastName;
        user.UserName = registerDto.UserName;

        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if(result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }
        return result.Errors;
    }

     public async Task<AuthResponseDto> Login(AuthDto loginDto)
    {
        bool isUserExist = false;
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        isUserExist = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (user == null || isUserExist == false)
        {
            return null;
        }
        
        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid) return null;
      
            // generate token then return data response.
            var token = await GenerateToken(user);
            return new AuthResponseDto
            {
                UserId=user.Id, Token=token
            };
    }

    // generate token for user:
    private async Task<string> GenerateToken(UserApi user)
    {
        // create signing key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
        
        // create crenditals
        var credentiais = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        // gather user information aka claims   
        var roles = await _userManager.GetRolesAsync(user);
        
        var roleClaims = roles.Select((role) => new Claim(ClaimTypes.Role, role));
        
        var userClaims = await _userManager.GetClaimsAsync(user);
        
        // generate new Claims:
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        }.Union(userClaims).Union(roleClaims);
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
            signingCredentials: credentiais
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public Task<AuthResponseDto> RefreshToken(string refreshToken)
    {
        throw new NotImplementedException();
    }
}