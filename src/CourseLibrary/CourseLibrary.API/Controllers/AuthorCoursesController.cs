using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;

namespace CourseLibrary.API.Controllers;

[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/courses")]
public class AuthorCoursesController : ControllerBase
{
    private readonly IValidator<CourseForCreationModel> _courseValidator;
    private readonly ICourseLibraryService _courseLibrary;
    private readonly IMapper _mapper;
    public AuthorCoursesController(ICourseLibraryService courseLibrary, IValidator<CourseForCreationModel> courseValidator, IMapper mapper)
    {
        _courseValidator = courseValidator ?? throw new ArgumentNullException(nameof(courseValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _courseLibrary = courseLibrary ?? throw new ArgumentNullException(nameof(courseLibrary));
    }


    /// <summary>
    /// Get author courses.
    /// </summary>
    /// <param name="authorId"> <see cref="Guid"/></param>
    /// <returns> <see cref="IEnumerable{CourseModel}"/></returns>
    /// <response code="200">Returns the <see cref="IEnumerable{CourseModel}"/> </response>
    /// <response code="400"><paramref name="authorId"/> is invalid.</response>
    /// <response code="404">Author is not found for provided <paramref name="authorId"/>.</response>
    [HttpGet("{authorId}", Name = "GetCoursesForAuthor")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
    [HttpCacheValidation(MustRevalidate = false)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CourseModel>>> Get(Guid authorId)
    {
        if (!await _courseLibrary.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var coursesForAuthorFromRepo = await _courseLibrary
            .GetCoursesAsync(authorId);
        return Ok(_mapper.Map<IEnumerable<CourseModel>>(coursesForAuthorFromRepo));
    }

    /// <summary>
    ///  Add course of author.
    /// </summary>
    /// <param name="authorId"><see cref="Guid"/></param>
    /// <param name="course"><see cref="CourseForCreationModel"/></param>
    /// <returns>A newly created <see cref="CourseModel"/></returns>
    /// <response code="201">Returns the newly created <see cref="CourseModel"/></response>
    /// <response code="400"><paramref name="course"/> is null or invalid.</response>
    ///  <response code="404">Author is not found for provided <paramref name="authorId"/>.</response>
    [HttpPost("{authorId}", Name = "CreateCourseForAuthor")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CourseModel>> Post(Guid authorId, CourseForCreationModel course)
    {

        var result = await _courseValidator.ValidateAsync(course);

        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);

            return BadRequest(ModelState);

        }

        if (!await _courseLibrary.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var courseEntity = _mapper.Map<Course>(course);
        await _courseLibrary.AddCourse(authorId, courseEntity);

        var courseToReturn = _mapper.Map<CourseModel>(courseEntity);

        return CreatedAtRoute("GetCoursesForAuthor",
            new { authorId },
            courseToReturn);
    }

    /// <summary>
    /// Add/Update course for provided author.
    /// </summary>
    /// <param name="authorId"><see cref="Guid"/></param>
    /// <param name="courseId"><see cref="Guid"/></param>
    /// <param name="course"><see cref="CourseForCreationModel"/></param>
    /// <returns>No content</returns>
    /// <response code="204">Course information updated.</response>
    /// <response code="404">Author is not found for provided <paramref name="authorId"/>.</response>
    /// <response code="400"><paramref name="course"/> is null or invalid.</response>
    [HttpPut("{authorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Put(Guid authorId, Guid courseId, CourseForCreationModel course)
    {
        var result = await _courseValidator.ValidateAsync(course);

        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);

            return BadRequest(ModelState);

        }

        if (!await _courseLibrary.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var courseForAuthorFromRepo = await _courseLibrary
            .GetCourseAsync(authorId, courseId);

        if (courseForAuthorFromRepo is null)
        {
            var courseToAdd = _mapper.Map<Course>(course);
            courseToAdd.Id = courseId;
            await _courseLibrary.AddCourse(authorId, courseToAdd);

            var courseToReturn = _mapper.Map<CourseModel>(courseToAdd);
            return CreatedAtRoute("GetCourseForAuthor",
                new { authorId, courseId = courseToReturn.Id },
                courseToReturn);
        }

        _mapper.Map(course, courseForAuthorFromRepo);

        await _courseLibrary.UpdateCourse(courseForAuthorFromRepo);

        return NoContent();
    }

    /// <summary>
    /// Delete course of provided author.
    /// </summary>
    /// <param name="authorId"><see cref="Guid"/></param>
    /// <param name="courseId"><see cref="Guid"/></param>
    /// <returns>No Content</returns>
    /// <response code="204">Author deleted.</response>
    /// <response code="400"><paramref name="authorId"/> or <paramref name="courseId"/> are invalid.</response>
    /// <response code="404">Author is not found for provided <paramref name="authorId"/> or Course is not found for provided <paramref name="courseId"/> .</response>
    [HttpDelete("{authorId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Delete(Guid authorId, Guid courseId)
    {
        if (!await _courseLibrary.AuthorExistsAsync(authorId))
        {
            return NotFound();
        }

        var courseForAuthorFromRepo = await _courseLibrary
            .GetCourseAsync(authorId, courseId);

        if (courseForAuthorFromRepo == null)
        {
            return NotFound();
        }

        await _courseLibrary.DeleteCourse(courseForAuthorFromRepo);

        return NoContent();
    }
}
