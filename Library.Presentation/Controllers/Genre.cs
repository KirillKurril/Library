using Library.Application.Common.Exceptions;
using Library.Application.GenreUseCases.Commands;
using Library.Application.GenreUseCases.Queries;
using Library.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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


        [HttpPost]
        public async Task<IActionResult> Create(
            string genreName,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateGenreCommand(genreName);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
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
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(
                   int id,
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
