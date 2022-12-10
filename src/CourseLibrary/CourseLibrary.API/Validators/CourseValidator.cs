
namespace CourseLibrary.API.Validators;

public class CourseValidator : AbstractValidator<CourseForCreationModel>
{

    public CourseValidator()
    {
        RuleFor(course => course.Title).NotNull().NotEmpty().Length(3, 100);
        RuleFor(course => course.Description).MaximumLength(300);
    }
}
