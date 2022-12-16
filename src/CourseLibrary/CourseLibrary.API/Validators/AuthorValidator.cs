
using FluentValidation;

namespace CourseLibrary.API.Validators;

public class AuthorValidator : AbstractValidator<AuthorForCreationModel>
{

    private readonly IValidator<CourseForCreationModel> _courseValidator;
    public AuthorValidator(IValidator<CourseForCreationModel> courseValidator)
    {
        _courseValidator = courseValidator ?? throw new ArgumentNullException(nameof(courseValidator));
        RuleFor(author => author.FirstName).NotNull().NotEmpty().Length(3, 50);
        RuleFor(author => author.LastName).NotNull().NotEmpty().Length(3, 50);
        RuleFor(author => author.DateOfBirth).Must(AgeHelper.BeValidAge).WithMessage("Invalid {PropertyName}");
        RuleFor(author => author.MainCategory).NotNull().NotEmpty();
        RuleForEach(author => author.Courses).SetValidator(_courseValidator);

    }
}
