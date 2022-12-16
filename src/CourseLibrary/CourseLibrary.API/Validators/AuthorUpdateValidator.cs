using FluentValidation;

namespace CourseLibrary.API.Validators;

public class AuthorUpdateValidator: AbstractValidator<AuthorForUpdateModel>
{
	public AuthorUpdateValidator()
	{
        RuleFor(author => author.FirstName).NotNull().NotEmpty().Length(3, 50);
        RuleFor(author => author.LastName).NotNull().NotEmpty().Length(3, 50);
        RuleFor(author => author.DateOfBirth).Must(AgeHelper.BeValidAge).WithMessage("Invalid {PropertyName}");
        RuleFor(author => author.MainCategory).NotNull().NotEmpty();

    }
}
