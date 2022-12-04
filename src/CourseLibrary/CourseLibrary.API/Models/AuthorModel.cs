namespace CourseLibrary.API.Models;

public record AuthorModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public int Age { get; init; }
    public string MainCategory { get; init; } = default!;
}
