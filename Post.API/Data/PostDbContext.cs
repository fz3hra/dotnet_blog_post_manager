using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Post.API.Data.Configuration;

namespace Post.API.Data;

public class PostDbContext: DbContext
{
    public PostDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new PostConfiguration());
    }
}