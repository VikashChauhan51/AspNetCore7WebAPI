
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;


namespace CourseLibrary.API.Controllers;

[ApiController]
[Authorize]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    /// <remarks>
    /// Sample request:
    /// 
    ///     Get /authorcollections/(3fa85f64-5717-4562-b3fc-2c963f66afa6,3fa85f64-5717-4562-b3fc-2c963f66afa7)
    ///     
    /// </remarks>
    /// <response code="200">Returns the <see cref="IEnumerable{AuthorModel}"/> information.</response>
    /// <response code="400"><paramref name="authorIds"/> are invalid.</response>
    /// <response code="404">Authors are not found for provided <paramref name="authorIds"/>.</response>
    [HttpGet("({authorIds})", Name = "GetAuthorCollection")]
    [HttpHead]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AuthorModel>>> Get([ModelBinder(BinderType = typeof(ArrayModelBinder))][FromRoute] IEnumerable<Guid> authorIds)
    {
        var authorEntities = await _courseLibrary
            .GetAuthorsAsync(authorIds);

        if (authorIds.Count() != authorEntities.Count())
        {
            return NotFound();
        }

        // map
        var authorsToReturn = _mapper.Map<IEnumerable<AuthorModel>>(authorEntities);
        var authorsWithLinks = CreateLinksForAuthors(authorsToReturn);
        var links = CreateLinksForAuthors(authorIds, false);
        var response = new LinkCollectionWrapper<LinkWrapper<AuthorModel>>
        {
            Value = authorsWithLinks,
            Links = links

        };
        return Ok(response);
    }

    /// <summary>
    ///  Add Authors.
    /// </summary>
    /// <param name="authorCollection"><see cref="IEnumerable{AuthorForCreationModel}"/></param>
    /// <returns>Returns the newly created Authors</returns>
    /// <response code="201">Returns the newly created Authors</response>
    /// <response code="400"><see cref="AuthorForCreationModel"/> are null or invalid.</response>

    [HttpPost(Name = "AddAuthorCollection")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<AuthorModel>>> Post([FromBody] IEnumerable<AuthorForCreationModel> authorCollection)
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
        var authorsWithLinks = CreateLinksForAuthors(authorCollectionToReturn);
        var links = CreateLinksForAuthors(authorCollectionToReturn.Select(a => a.Id).ToList(), true);
        var response = new LinkCollectionWrapper<LinkWrapper<AuthorModel>>
        {
            Value = authorsWithLinks,
            Links = links

        };
        return CreatedAtRoute("GetAuthorCollection",
          new { authorIds = authorIdsAsString },
          response);
    }

    private IEnumerable<LinkWrapper<AuthorModel>> CreateLinksForAuthors(IEnumerable<AuthorModel> authors)
    {
        var list = new List<LinkWrapper<AuthorModel>>();
        foreach (var author in authors)
        {
            var authorLinks = CreateLinksForAuthor(author.Id);
            list.Add(new LinkWrapper<AuthorModel>
            {
                Value = author,
                Links = authorLinks
            });

        }

        return list;

    }

    private IEnumerable<Link> CreateLinksForAuthor(Guid authorId)
    {
        return new List<Link> {
         new Link
         {
             Href=Url.Link("GetAuthor",  values: new { authorId })!,
             Rel= "self",
             Method= "GET"
         },
        new Link
        {
            Href=Url.Link("AddAuthor",null)!,
             Rel="create_author",
             Method="POST"
        },
        new Link
        {
            Href= Url.Link("UpdateAuthor", values : new { authorId })!,
              Rel="update_author",
              Method="PATCH"
        },
        new Link
        {
            Href= Url.Link("DeleteAuthor",  values : new { authorId })!,
             Rel= "delete_author",
              Method="DELETE"
        }

        };

    }

    private IEnumerable<Link> CreateLinksForAuthors(IEnumerable<Guid> authorIds, bool AddIds)
    {
        var links = new List<Link>();
        var href = Url.Link("GetAuthorCollection", values: AddIds ? new { authorIds } : null)!;
        var authorIdsAsString = string.Join(",", authorIds.Select(a => a));
        links.Add(new Link
        {
            Href = $"{href.Split('(')[0]}({authorIdsAsString})",
            Rel = "self",
            Method = "GET"
        });

        links.Add(new Link
        {
            Href = Url.Link("AddAuthorCollection", null)!,
            Rel = "create_authors",
            Method = "POST"
        });

        return links;

    }

}
