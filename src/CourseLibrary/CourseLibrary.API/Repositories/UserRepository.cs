using CourseLibrary.API.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Repositories;

public class UserRepository : RepositoryBase, IUserRepository
{
    public UserRepository(CourseLibraryContext context) : base(context)
    {

    }

    public async Task<User?> GetUserAsync(string email, string password)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new ArgumentNullException(nameof(email));
        }

        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password));
        }

        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

    }
    public async Task<User?> GetUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentNullException(nameof(userId));
        }
        return await _context.Users.FirstOrDefaultAsync(a => a.Id == userId);
    }
}
