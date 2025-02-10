using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace User.API.Data;

/// <summary>
/// Represents the database context for user authentication.
/// </summary>
public class UserDbContext : IdentityDbContext<UserApi>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserDbContext"/> class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public UserDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <summary>
    /// Configures the database model.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}