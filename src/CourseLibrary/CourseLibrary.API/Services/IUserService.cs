namespace CourseLibrary.API.Services
{
    public interface IUserService
    {
        Task<User?> GetUser(Guid userId);
        Task<User?> Login(string email, string password);
    }
}