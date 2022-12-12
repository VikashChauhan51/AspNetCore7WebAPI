namespace CourseLibrary.API.Models;
public record AuthorForCreationWithDateOfDeathModel: AuthorForCreationModel
{
    public DateTime? DateOfDeath { get; init; }
}
