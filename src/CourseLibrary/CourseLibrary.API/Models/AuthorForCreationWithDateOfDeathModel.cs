namespace CourseLibrary.API.Models;
public class AuthorForCreationWithDateOfDeathModel: AuthorForCreationModel
{
    public DateTimeOffset? DateOfDeath { get; set; }
}
