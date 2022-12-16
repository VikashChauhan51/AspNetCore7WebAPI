namespace CourseLibrary.API.Models;

public record AuthorForUpdateModel(
    string FirstName,
    string LastName,
    string MainCategory,
    DateTime DateOfBirth
    );

