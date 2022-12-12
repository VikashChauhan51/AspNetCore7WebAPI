
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Entities;

public class Course
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;   

    public string? Description { get; set; }

    public Author Author { get; set; } = null!;

    public Guid AuthorId { get; set; }
}


public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(c => c.Id).IsRequired();
        builder.Property(c => c.Title).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Description).HasMaxLength(300);
        builder.HasKey(c => c.Id);
        builder.Property(c => c.AuthorId).IsRequired();
        builder.HasOne(c => c.Author).WithMany(c=>c.Courses).HasForeignKey(c=>c.AuthorId).IsRequired();
        builder.Navigation(c => c.Author);
        builder.HasIndex(c => c.AuthorId);
        builder.ToTable("Courses");
    }
}