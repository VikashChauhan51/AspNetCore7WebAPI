
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/author")]
    public class AuthorController : ControllerBase
    {

        private readonly IValidator<AuthorModel> _authorValidator;
        private readonly ICourseLibraryService _courseLibrary;
        private readonly IMapper _mapper;
        public AuthorController(IValidator<AuthorModel> authorValidator, IMapper mapper, ICourseLibraryService courseLibrary)
        {
            _authorValidator = authorValidator ?? throw new ArgumentNullException(nameof(authorValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _courseLibrary = courseLibrary ?? throw new ArgumentNullException(nameof(courseLibrary));
        }


        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthorModel>> Get( Guid authorId)
        {
            Author? authorFromRepo = null;// await _courseLibrary.GetAuthorAsync(authorId);

            if (authorFromRepo == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorModel>(authorFromRepo));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthorModel>> Post(AuthorModel author)
        {
            var result = await _authorValidator.ValidateAsync(author);

            if (!result.IsValid)
            {
                result.AddToModelState(this.ModelState);

                return BadRequest(ModelState);
            }

            var authorEntity = _mapper.Map<Author>(author);

            var authorToReturn = _mapper.Map<AuthorModel>(authorEntity);

            return CreatedAtRoute("GetAuthor",
                new { authorId = authorToReturn.Id },
                authorToReturn);
        }
    }
}
