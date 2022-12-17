
using Marvin.Cache.Headers;

namespace CourseLibrary.API.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/course")]
public class CourseController : ControllerBase
{
    private readonly IValidator<CourseForCreationModel> _courseValidator;
    private readonly ICourseLibraryService _courseLibrary;
    private readonly IMapper _mapper;
    public CourseController(ICourseLibraryService courseLibrary, IValidator<CourseForCreationModel> courseValidator, IMapper mapper)
    {
        _courseValidator = courseValidator ?? throw new ArgumentNullException(nameof(courseValidator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _courseLibrary = courseLibrary ?? throw new ArgumentNullException(nameof(courseLibrary));

    }

    /// <summary>
    /// Get Course information.
    /// </summary>
    /// <param name="courseId"><see cref="Guid"/></param>
    /// <returns>Returns the <see cref="CourseModel"/> information.</returns>
    /// <response code="200">Returns the <see cref="CourseModel"/> information.</response>
    /// <response code="400"><paramref name="courseId"/> is invalid.</response>
    /// <response code="404">Author is not found for provided <paramref name="courseId"/>.</response>
    [HttpGet("{courseId}", Name = "GetCourse")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
    [HttpCacheValidation(MustRevalidate = false)]
    public async Task<ActionResult<CourseModel>> Get(Guid courseId)
    {
        Course? courseFromRepo = await _courseLibrary.GetCourseAsync(courseId);

        if (courseFromRepo is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CourseModel>(courseFromRepo));
    }
    /// <summary>
    ///  Update course information.
    /// </summary>
    /// <param name="courseId"><see cref="Guid"/></param>
    /// <param name="patchDocument"><see cref="JsonPatchDocument{CourseForCreationModel}"/></param>
    /// <returns>No content.</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///               [
    ///                 {
    ///                     "operationType": 2,
    ///                     "path": "/title",
    ///                     "op": "replace",
    ///                     "from": "Basic of Appium with c#",
    ///                     "value": "Basic of Appium"
    ///                 },
    ///                 {
    ///                     "operationType": 2,
    ///                     "path": "/description",
    ///                     "op": "replace",
    ///                     "value": "Basic of Appium with C#"
    ///                 }
    ///              ]
    /// 
    /// </remarks>
    /// <response code="204">Course information updated.</response>
    /// <response code="404">Course is not found for provided <paramref name="courseId"/>.</response>
    /// <response code="400"><see cref="CourseForCreationModel"/> is null or invalid.</response>
    [HttpPatch("{courseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Patch(Guid courseId, [FromBody] JsonPatchDocument<CourseForCreationModel> patchDocument)
    {
        Course? courseFromRepo = await _courseLibrary.GetCourseAsync(courseId);

        if (courseFromRepo is null)
        {
            return NotFound();
        }

        var courseToPatch = _mapper.Map<CourseForCreationModel>(
            courseFromRepo);
        patchDocument.ApplyTo(courseToPatch, ModelState);

        var result = await _courseValidator.ValidateAsync(courseToPatch);

        if (!result.IsValid)
        {
            result.AddToModelState(this.ModelState);

            return BadRequest(ModelState);

        }

        _mapper.Map(courseToPatch, courseFromRepo);

        await _courseLibrary.UpdateCourse(courseFromRepo);

        return NoContent();
    }
}
