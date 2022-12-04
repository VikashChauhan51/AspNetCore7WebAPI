using CourseLibrary.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/course")]
    public class CourseController : ControllerBase
    {
        [HttpGet(Name = "GetCourse")]
        public CourseModel Get()
        {
            return new();
        }
    }
}
