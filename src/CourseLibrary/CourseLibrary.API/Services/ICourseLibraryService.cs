using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Services;

public interface ICourseLibraryService
{
    Task<IEnumerable<Course>> GetCoursesAsync(Guid authorId);
    Task<Course?> GetCourseAsync( Guid courseId);
    Task AddCourse(Guid authorId, Course course);
    Task UpdateCourse(Course course);
    Task DeleteCourse(Course course);
    Task<IEnumerable<Author>> GetAuthorsAsync();
    Task<Author?> GetAuthorAsync(Guid authorId);
    Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds);
    Task AddAuthor(Author author);
    Task DeleteAuthor(Author author);
    Task UpdateAuthor(Author author);
    Task<bool> AuthorExistsAsync(Guid authorId);
}
