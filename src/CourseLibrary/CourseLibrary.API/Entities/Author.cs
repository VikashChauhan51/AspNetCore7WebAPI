
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;

namespace CourseLibrary.API.Entities;

public class Author
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateTimeOffset DateOfBirth { get; set; }
    public string MainCategory { get; set; } = string.Empty;

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.Property(a => a.Id).IsRequired();
        builder.Property(a => a.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(a => a.LastName).IsRequired().HasMaxLength(50);
        builder.HasKey(a => a.Id);
        builder.Property(a => a.DateOfBirth).IsRequired();
        builder.Property(a => a.MainCategory).IsRequired().HasMaxLength(50);
        builder.HasMany(a => a.Courses).WithOne();
    }
}