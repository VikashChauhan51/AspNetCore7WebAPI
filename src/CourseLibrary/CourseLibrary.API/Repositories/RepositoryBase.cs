using CourseLibrary.API.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Repositories;

public abstract class RepositoryBase: IRepository
{
    protected readonly CourseLibraryContext _context;

    public RepositoryBase(CourseLibraryContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public virtual async Task<bool> SaveAsync()
    {
        return (await _context.SaveChangesAsync() >= 0);
    }
}
