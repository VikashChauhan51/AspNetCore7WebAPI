namespace CourseLibrary.API.Models;

public record AuthorFullModel
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string MainCategory { get; init; } = string.Empty;
}
