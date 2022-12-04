using CourseLibrary.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.DbContexts;

public class CourseLibraryContext : DbContext
{
    public CourseLibraryContext(DbContextOptions<CourseLibraryContext> options)
       : base(options)
    {
    }

    // base DbContext constructor ensures that Books and Authors are not null after
    // having been constructed.  Compiler warning ("uninitialized non-nullable property")
    // can safely be ignored with the "null-forgiving operator" (= null!)

    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Course> Courses { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthorConfiguration).Assembly);
    }
}
