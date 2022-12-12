namespace CourseLibrary.API.Models;

public record AuthorFullModel
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string MainCategory { get; set; } = string.Empty;
}
