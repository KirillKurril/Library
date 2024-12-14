using Library.Application.AuthorUseCases.Commands;
using Library.Application.AuthorUseCases.Queries;
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
            var query = new GetAllAuthorsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("for-filtration")]
        public async Task<ActionResult<IEnumerable<AuthorBriefDTO>>> GetForFiltrationList(CancellationToken cancellationToken)
        {
            var query = new GetAllAuthorsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<ActionResult<Author>> GetById(Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetAuthorByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return NotFound($"Author with ID {id} not found");
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(CreateAuthorDTO createAuthorDTO,
            CancellationToken cancellationToken)
        {
            var command = createAuthorDTO.Adapt<CreateAuthorCommand>();
            var response = await _mediator.Send(command, cancellationToken);
            response.RedirectUrl = Url.Action(nameof(GetById), new { id = response.Id });
            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(UpdateBookDTO updateAuthorDTO,
            CancellationToken cancellationToken)
        {
            var command = updateAuthorDTO.Adapt<UpdateAuthorCommand>();
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteAuthorCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
