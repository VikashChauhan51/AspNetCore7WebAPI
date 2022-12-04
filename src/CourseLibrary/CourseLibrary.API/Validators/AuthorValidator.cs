
namespace CourseLibrary.API.Validators;

public class AuthorValidator : AbstractValidator<AuthorModel>
{
    public AuthorValidator()
    {
        RuleFor(author => author.Name).NotNull().NotEmpty().Length(3, 50);
        RuleFor(author => author.Age).GreaterThanOrEqualTo(18).LessThanOrEqualTo(80);
        RuleFor(author => author.MainCategory).NotNull().NotEmpty();
    }
}
