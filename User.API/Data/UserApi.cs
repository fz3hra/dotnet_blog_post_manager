using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace User.API.Data;

/// <summary>
/// Represents a user in the authentication system.
/// </summary>
public class UserApi : IdentityUser
{
    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    [Required]
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    [Required]
    public string LastName { get; set; }
}