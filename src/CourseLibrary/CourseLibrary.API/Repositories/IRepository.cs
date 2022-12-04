namespace CourseLibrary.API.Repositories;

public interface IRepository
{
    Task<bool> SaveAsync();
}
