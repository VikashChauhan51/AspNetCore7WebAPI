using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Repositories
{
    public interface IAuthorRepository: IRepository
    {
        void AddAuthor(Author author);
        Task<bool> AuthorExistsAsync(Guid authorId);
        void DeleteAuthor(Author author);
        Task<Author?> GetAuthorAsync(Guid authorId);
        Task<IEnumerable<Author>> GetAuthorsAsync();
        Task<IEnumerable<Author>> GetAuthorsAsync(IEnumerable<Guid> authorIds);
        void UpdateAuthor(Author author);
    }
}