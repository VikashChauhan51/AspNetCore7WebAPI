namespace CourseLibrary.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public Task<User?> Login(string email, string password)
    {
        return _userRepository.GetUserAsync(email, password);
    }
    public Task<User?> GetUser(Guid userId)
    {
        return _userRepository.GetUserAsync(userId);
    }
}
