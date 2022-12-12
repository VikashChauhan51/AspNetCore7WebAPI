namespace CourseLibrary.API.Models;

public record AuthorForCreationModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string MainCategory { get; set; } = string.Empty;
    public ICollection<CourseForCreationModel> Courses { get; set; } = new List<CourseForCreationModel>();
}
