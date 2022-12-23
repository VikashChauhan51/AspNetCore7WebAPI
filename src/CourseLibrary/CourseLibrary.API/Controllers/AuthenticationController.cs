using CourseLibrary.API.Configurations;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Validators;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CourseLibrary.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AuthenticationConfiguration _authenticationConfiguration;
    private readonly IValidator<AuthenticationModel> _authenticationValidator;
    public AuthenticationController(IUserService userService, IOptions<AuthenticationConfiguration> options, IValidator<AuthenticationModel> authenticationValidator)
    {

        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _authenticationConfiguration = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _authenticationValidator = authenticationValidator ?? throw new ArgumentNullException(nameof(authenticationValidator));
    }
    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<string>> Post(AuthenticationModel authentication)
    {
        var result = await _authenticationValidator.ValidateAsync(authentication);

        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);

            return BadRequest(ModelState);
        }
        var user = await _userService.Login(authentication.Email, authentication.Password);

        if (user == null)
        {
            return Unauthorized();
        }
        // Step 2: create a token
        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_authenticationConfiguration.SecretForKey));
        var signingCredentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claimsForToken = new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim("given_name", user.FirstName),
            new Claim("family_name", user.LastName),
            new Claim("email", user.Email)
        };

        var jwtSecurityToken = new JwtSecurityToken(
            _authenticationConfiguration.Issuer,
            _authenticationConfiguration.Audience,
            claimsForToken,
            DateTime.UtcNow,
            DateTime.UtcNow.AddHours(5),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler()
           .WriteToken(jwtSecurityToken);

        return Ok(tokenToReturn);
    }
}
