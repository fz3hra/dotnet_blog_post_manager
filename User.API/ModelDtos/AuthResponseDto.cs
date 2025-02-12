namespace User.API.Data;

public class AuthResponseDto
{
    public string UserId { get; set; }
    public string Token { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
}