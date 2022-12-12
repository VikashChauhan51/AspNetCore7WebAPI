namespace CourseLibrary.API.Models;

public record AuthorForCreationModel
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public DateTime DateOfBirth { get; init; }
    public string MainCategory { get; init; } = default!;   
    public ICollection<CourseForCreationModel> Courses { get; init; } = new List<CourseForCreationModel>();
}
