using Library.Application.Common.Exceptions;
using Library.Application.GenreUseCases.Commands;
using Library.Application.GenreUseCases.Queries;
using Library.Domain.Entities;
using MediatR;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Library.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GenreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Genre>>> GetAllList(
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllGenresQuery();
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving all genres. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Genre>> GetById(
           Guid id,
           CancellationToken cancellationToken)
        {
            try
            {
                var command = new GetGenreByIdQuery(id);
                var genre = await _mediator.Send(command, cancellationToken);
                return Ok(genre);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving genre with ID {id}. {ex.Message}");
            }
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<CreateEntityResponse>> Create(
            string genreName,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = genreName.Adapt<CreateGenreCommand>();
                var response = await _mediator.Send(command, cancellationToken);
                response.RedirectUrl = Url.Action(nameof(GetById), new { id = response.Id });
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the genre. {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(
                   Guid id,
                   CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteGenreCommand(id);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting genre with ID {id}. {ex.Message}");
            }
        }
    }
}
