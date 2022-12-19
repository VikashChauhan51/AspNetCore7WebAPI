

namespace CourseLibrary.API.Controllers;


[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
public class RootController : ControllerBase
{
    [HttpGet(Name = "GetRoot")]
    public IActionResult GetRoot()
    {
        // create links for root
        var links = new List<Link>
        {
            new Link(Url.Link("GetRoot", new { })!,
          "self",
          "GET"),

            new Link(Url.Link("AddAuthor", new { })!,
        "create_author",
        "POST"),

            new Link(Url.Link("AddAuthorCollection", new { })!,
        "create_authors",
        "POST")
        };

        return Ok(links);
    }
}
