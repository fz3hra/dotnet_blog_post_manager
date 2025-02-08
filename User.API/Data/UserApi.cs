using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace User.API.Data;

public  class UserApi: IdentityUser
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
}