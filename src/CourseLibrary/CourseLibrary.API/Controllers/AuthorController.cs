
using Marvin.Cache.Headers;

namespace CourseLibrary.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/author")]
public class AuthorController : ControllerBase
{

    private readonly IValidator<AuthorForCreationModel> _authorValidator;
    private readonly IValidator<AuthorForUpdateModel> _authorUpdateValidator;
    private readonly ICourseLibraryService _courseLibrary;
    private readonly IMapper _mapper;
    public AuthorController(IValidator<AuthorForCreationModel> authorValidator,
        IMapper mapper,
        ICourseLibraryService courseLibrary,
        IValidator<AuthorForUpdateModel> authorUpdateValidator)
    {
        _authorValidator = authorValidator ?? throw new ArgumentNullException(nameof(authorValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _courseLibrary = courseLibrary ?? throw new ArgumentNullException(nameof(courseLibrary));
        _authorUpdateValidator = authorUpdateValidator ?? throw new ArgumentNullException(nameof(authorUpdateValidator));
    }

    /// <summary>
    /// Get Author information.
    /// </summary>
    /// <param name="authorId"><see cref="Guid"/></param>
    /// <returns>Returns the <see cref="AuthorModel"/> information.</returns>
    /// <response code="200">Returns the <see cref="AuthorModel"/> information.</response>
    /// <response code="400"><paramref name="authorId"/> is invalid.</response>
    /// <response code="404">Author is not found for provided <paramref name="authorId"/>.</response>
    [HttpGet("{authorId}", Name = "GetAuthor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
    [HttpCacheValidation(MustRevalidate = false)]
    public async Task<ActionResult<AuthorModel>> Get(Guid authorId)
    {
        Author? authorFromRepo = await _courseLibrary.GetAuthorAsync(authorId);

        if (authorFromRepo is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<AuthorModel>(authorFromRepo));
    }

    /// <summary>
    /// Add new Author with Courses.
    /// </summary>
    /// <param name="author"><see cref="AuthorForCreationModel"/></param>
    /// <returns>A newly created <see cref="AuthorModel"/></returns>
    /// <response code="201">Returns the newly created <see cref="CourseModel"/></response>
    /// <response code="400"><paramref name="author"/> is null or invalid.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorModel>> Post([FromBody] AuthorForCreationModel author)
    {
        var result = await _authorValidator.ValidateAsync(author);

        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);

            return BadRequest(ModelState);
        }

        var authorEntity = _mapper.Map<Author>(author);
        await _courseLibrary.AddAuthor(authorEntity);

        var authorToReturn = _mapper.Map<AuthorModel>(authorEntity);

        return CreatedAtRoute("GetAuthor",
            new { authorId = authorToReturn.Id },
            authorToReturn);
    }

    /// <summary>
    /// Update Author information without Courses.
    /// </summary>
    /// <param name="authorId"><see cref="Guid"/></param>
    /// <param name="patchDocument"><see cref="JsonPatchDocument{AuthorForUpdateModel}"/></param>
    /// <returns>No content.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     Patch /Author
    ///     [
    ///              {
    ///                 "operationType": 2,
    ///                 "path": "/firstName",
    ///                 "op": "replace",
    ///                 "value": "updated firstName value"
    ///              },
    ///              {
    ///                 "operationType": 2,
    ///                  "path": "/lastName",
    ///                  "op": "replace",
    ///                  "value": "updated lastname value"
    ///              },
    ///              {
    ///                 "operationType": 2,
    ///                 "path": "/mainCategory",
    ///                 "op": "replace",
    ///                 "value": "updated mainCategory value"
    ///              },
    ///              {
    ///                 "operationType": 2,
    ///                 "path": "/dateOfBirth",
    ///                 "op": "replace",
    ///                 "value": "2022-12-16T15:09:23.663Z"
    ///              }
    ///      ]   
    ///      
    /// </remarks>
    /// <response code="204">Author information updated.</response>
    /// <response code="404">Author is not found for provided <paramref name="authorId"/>.</response>
    /// <response code="400"><see cref="AuthorForUpdateModel"/> is null or invalid.</response>
    [HttpPatch("{authorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Patch(Guid authorId, [FromBody] JsonPatchDocument<AuthorForUpdateModel> patchDocument)
    {


        Author? authorFromRepo = await _courseLibrary.GetAuthorAsync(authorId);

        if (authorFromRepo is null)
        {
            return NotFound();
        }

        var authorToPatch = _mapper.Map<AuthorForUpdateModel>(
            authorFromRepo);
        patchDocument?.ApplyTo(authorToPatch, ModelState);

        var result = await _authorUpdateValidator.ValidateAsync(authorToPatch);
        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);

            return BadRequest(ModelState);
        }

        _mapper.Map(authorToPatch, authorFromRepo);

        await _courseLibrary.UpdateAuthor(authorFromRepo);

        return NoContent();
    }

    /// <summary>
    /// Remove Author with Courses.
    /// </summary>
    /// <param name="authorId"><see cref="Guid"/></param>
    /// <returns>No content.</returns>
    /// <response code="204">Author deleted.</response>
    /// <response code="400"><paramref name="authorId"/> is invalid.</response>
    /// <response code="404">Author is not found for provided <paramref name="authorId"/>.</response>
    [HttpDelete("{authorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid authorId)
    {
        Author? authorFromRepo = await _courseLibrary.GetAuthorAsync(authorId);

        if (authorFromRepo is null)
        {
            return NotFound();
        }

        await _courseLibrary.DeleteAuthor(authorFromRepo);

        return NoContent();
    }

}
