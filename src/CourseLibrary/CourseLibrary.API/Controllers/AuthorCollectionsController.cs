
using Marvin.Cache.Headers;

namespace CourseLibrary.API.Controllers;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/authorcollections")]
public class AuthorCollectionsController : ControllerBase
{
    private readonly IValidator<AuthorForCreationModel> _authorValidator;
    private readonly ICourseLibraryService _courseLibrary;
    private readonly IMapper _mapper;
    public AuthorCollectionsController(IValidator<AuthorForCreationModel> authorValidator,
        IMapper mapper,
        ICourseLibraryService courseLibrary)
	{
        _authorValidator = authorValidator ?? throw new ArgumentNullException(nameof(authorValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _courseLibrary = courseLibrary ?? throw new ArgumentNullException(nameof(courseLibrary));

    }

    /// <summary>
    ///  Get Authors.
    /// </summary>
    /// <param name="authorIds"><see cref="IEnumerable{Guid}"/></param>
    /// <returns>Returns the <see cref="IEnumerable{AuthorModel}"/> information.</returns>
    /// <response code="200">Returns the <see cref="IEnumerable{AuthorModel}"/> information.</response>
    /// <response code="400"><paramref name="authorIds"/> are invalid.</response>
    /// <response code="404">Authors are not found for provided <paramref name="authorIds"/>.</response>
    [HttpGet("({authorIds})", Name = "GetAuthorCollection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
    [HttpCacheValidation(MustRevalidate = false)]
    public async Task<ActionResult<IEnumerable<AuthorModel>>>Get([FromRoute] IEnumerable<Guid> authorIds)
    {
        var authorEntities = await _courseLibrary
            .GetAuthorsAsync(authorIds);

        if (authorIds.Count() != authorEntities.Count())
        {
            return NotFound();
        }

        // map
        var authorsToReturn = _mapper.Map<IEnumerable<AuthorModel>>(authorEntities);
        return Ok(authorsToReturn);
    }

    /// <summary>
    ///  Add Authors.
    /// </summary>
    /// <param name="authorCollection"><see cref="IEnumerable{AuthorForCreationModel}"/></param>
    /// <returns>Returns the newly created Authors</returns>
    /// <response code="201">Returns the newly created Authors</response>
    /// <response code="400"><see cref="AuthorForCreationModel"/> are null or invalid.</response>

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<AuthorModel>>> Post([FromBody]IEnumerable<AuthorForCreationModel> authorCollection)
    {
        if (!authorCollection?.Any() ?? false)
        {
            return BadRequest(ModelState);
        }

        foreach (var author in authorCollection!)
        {
            var result = await _authorValidator.ValidateAsync(author);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);

                return BadRequest(ModelState);
            }
        }

        var authorEntities = _mapper.Map<IEnumerable<Author>>(authorCollection);
        await _courseLibrary.AddAuthors(authorEntities);

        var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorModel>>(authorEntities);
        var authorIdsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));

        return CreatedAtRoute("GetAuthorCollection",
          new { authorIds = authorIdsAsString },
          authorCollectionToReturn);
    }
}
