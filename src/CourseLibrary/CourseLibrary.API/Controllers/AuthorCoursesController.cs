using Marvin.Cache.Headers;

namespace CourseLibrary.API.Controllers;

[ApiController]
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



    [HttpGet("{authorId}", Name = "GetCoursesForAuthor")]
    [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 1000)]
    [HttpCacheValidation(MustRevalidate = false)]
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

    [HttpPost("{authorId}", Name = "CreateCourseForAuthor")]
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

    [HttpPut("{authorId}")]
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

    [HttpDelete("{authorId}")]
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
