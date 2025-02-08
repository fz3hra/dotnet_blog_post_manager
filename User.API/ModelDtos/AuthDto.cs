using System.ComponentModel.DataAnnotations;

namespace User.API.ModelDtos;

public class AuthDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [StringLength(15, ErrorMessage = "Your password must be between 6 and 15 characters long.")]
    public string Password { get; set; }
}