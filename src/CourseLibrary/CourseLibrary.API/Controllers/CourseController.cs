using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{

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

        [HttpGet("{courseId}", Name = "GetCourse")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CourseModel>> Get(Guid courseId)
        {
            Course? courseFromRepo = await _courseLibrary.GetCourseAsync(courseId);

            if (courseFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CourseModel>(courseFromRepo));
        }
    }
}
