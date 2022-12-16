
using CourseLibrary.API.Entities;
using CourseLibrary.API.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CourseLibrary.API.Controllers
{
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


        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthorModel>> Get( Guid authorId)
        {
            Author? authorFromRepo =  await _courseLibrary.GetAuthorAsync(authorId);

            if (authorFromRepo is null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<AuthorModel>(authorFromRepo));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthorModel>> Post(AuthorForCreationModel author)
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

        [HttpPatch("{authorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Patch(Guid authorId, JsonPatchDocument<AuthorForUpdateModel> patchDocument)
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

        [HttpDelete("{authorId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
}
