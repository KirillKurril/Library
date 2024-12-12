using Library.Application.AuthorUseCases.Commands;
using Library.Application.AuthorUseCases.Queries;
using Library.Application.BookUseCases.Commands;
using Library.Application.Common.Exceptions;
using Library.Application.DTOs;
using Library.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Author>>> GetAllList(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllAuthorsQuery();
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving all authors list. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("for-filtration")]
        public async Task<ActionResult<IEnumerable<AuthorBriefDTO>>> GetForFiltrationList(CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAllAuthorsQuery();
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving all authors list. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<ActionResult<Author>> GetById(Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetAuthorByIdQuery(id);
                var result = await _mediator.Send(query, cancellationToken);

                if (result == null)
                    return NotFound($"Author with ID {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving author with ID {id}. {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateAuthorDTO createAuthorDTO,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = createAuthorDTO.Adapt<CreateAuthorCommand>();
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
                return StatusCode(500, $"An error occurred while creating the author. {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(UpdateBookDTO updateAuthorDTO,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = updateAuthorDTO.Adapt<UpdateAuthorCommand>();
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the author. {ex.Message}");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteAuthorCommand(id);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting author with ID {id}");
            }
        }
    }
}
