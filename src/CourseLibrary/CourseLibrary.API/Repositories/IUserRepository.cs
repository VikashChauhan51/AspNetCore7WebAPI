namespace CourseLibrary.API.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(Guid userId);
        Task<User?> GetUserAsync(string email, string password);
    }
}