using System.ComponentModel.DataAnnotations;

namespace User.API.ModelDtos;

public class RegisterDto: AuthDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string UserName { get; set; }
}