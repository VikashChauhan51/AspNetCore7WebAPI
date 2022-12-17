using CourseLibrary.API.Entities;
using CourseLibrary.API.Repositories;

namespace CourseLibrary.API.Services;

public class CourseLibraryService : ICourseLibraryService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ICourseRepository _courseRepository;
    public CourseLibraryService(IAuthorRepository authorRepository, ICourseRepository courseRepository)
    {
        _authorRepository = authorRepository ?? throw new ArgumentNullException(nameof(authorRepository));
        _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
    }

    public async Task AddAuthors(IEnumerable<Author> authors)
    {
        foreach (var author in authors)
        {
            _authorRepository.AddAuthor(author);
        }
        await _authorRepository.SaveAsync();
    }

    public Task<Course?> GetCourseAsync(Guid authorId, Guid courseId)
    {
        return _courseRepository.GetCourseAsync(authorId, courseId);
    }

    async Task ICourseLibraryService.AddAuthor(Author author)
    {
       
        _authorRepository.AddAuthor(author);
        await _authorRepository.SaveAsync();
    }

    async Task ICourseLibraryService.AddCourse(Guid authorId, Course course)
    {
        _courseRepository.AddCourse(authorId, course);
        await _courseRepository.SaveAsync();    
    }

    Task<bool> ICourseLibraryService.AuthorExistsAsync(Guid authorId)
    {
        return _authorRepository.AuthorExistsAsync(authorId);
    }

    async Task ICourseLibraryService.DeleteAuthor(Author author)
    {
        _authorRepository.DeleteAuthor(author);
        await _authorRepository.SaveAsync();    
    }

    async Task ICourseLibraryService.DeleteCourse(Course course)
    {
        _courseRepository.DeleteCourse(course);
        await _courseRepository.SaveAsync();    
    }

    Task<Author?> ICourseLibraryService.GetAuthorAsync(Guid authorId)
    {
        return _authorRepository.GetAuthorAsync(authorId);
    }

    Task<IEnumerable<Author>> ICourseLibraryService.GetAuthorsAsync()
    {
        return _authorRepository.GetAuthorsAsync();
    }

    Task<IEnumerable<Author>> ICourseLibraryService.GetAuthorsAsync(IEnumerable<Guid> authorIds)
    {
        return _authorRepository.GetAuthorsAsync(authorIds);
    }

    Task<Course?> ICourseLibraryService.GetCourseAsync( Guid courseId)
    {
        return _courseRepository.GetCourseAsync(courseId);
    }

    Task<IEnumerable<Course>> ICourseLibraryService.GetCoursesAsync(Guid authorId)
    {
        return _courseRepository.GetCoursesAsync(authorId);
    }

    async Task ICourseLibraryService.UpdateAuthor(Author author)
    {
        _authorRepository.UpdateAuthor(author);
       await _authorRepository.SaveAsync();
    }

    async Task ICourseLibraryService.UpdateCourse(Course course)
    {
        _courseRepository.UpdateCourse(course);
        await _courseRepository.SaveAsync();
    }
}
