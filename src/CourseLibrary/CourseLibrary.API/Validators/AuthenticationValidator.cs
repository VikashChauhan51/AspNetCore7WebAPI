namespace CourseLibrary.API.Validators;

public class AuthenticationValidator : AbstractValidator<AuthenticationModel>
{
    public AuthenticationValidator()
    {
        RuleFor(author => author.Email).NotNull().NotEmpty().Length(8, 50).EmailAddress();
        RuleFor(author => author.Password).NotNull().NotEmpty().Length(8, 50);
    }
}
