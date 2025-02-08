using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Post.API.Data.Configuration;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(p => p.CreatedBy)
            .IsRequired();
        
        builder.Property(p => p.LastModifiedAt)
            .IsRequired(false);
        
        builder.Property(p => p.LastModifiedBy)
            .IsRequired(false);
        
        builder.Property(p => p.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);
        
        builder.Property(p => p.PublishedAt)
            .IsRequired(false);
        
        // Configure Tags as JSON string in the database
        builder.Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null))
            .HasColumnType("nvarchar(max)")  // Set to max length
            .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()));
        
        builder.Property(p => p.FeaturedImageUrl)
            .IsRequired(false)
            .HasMaxLength(500);
        
        builder.Property(p => p.ViewCount)
            .IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(p => p.Excerpt)
            .IsRequired(false)
            .HasMaxLength(200);
        
        builder.Property(p => p.Status)
            .IsRequired()
            .HasDefaultValue(PostStatus.Draft)
            .HasConversion<string>();
        //
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.CreatedBy);
    }
}