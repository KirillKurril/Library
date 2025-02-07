using Library.Application.AuthorUseCases.Commands;
using Library.Application.AuthorUseCases.Queries;
using Library.Application.BookUseCases.Queries;
using Library.Application.Common.Models;
using Library.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Presentation.Controllers
{
    [Route("authors")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<Author>>> GetAllList(CancellationToken cancellationToken)
        {
            var query = new GetAllAuthorsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("filtred-list")]
        [ProducesResponseType(typeof(ResponseData<Author>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginationListModel<Author>>> GetFiltredList(
            [FromQuery] string? searchTerm,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            var query = new GetAuthorsListQuery(searchTerm, pageNo, itemsPerPage);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("for-filtration")]
        public async Task<ActionResult<IEnumerable<AuthorBriefDTO>>> GetForFiltrationList(CancellationToken cancellationToken)
        {
            var query = new GetAllForFiltrationAuthorsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<ActionResult<Author>> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetAuthorByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateAuthorDTO createAuthorDTO,
            CancellationToken cancellationToken)
        {
            var command = createAuthorDTO.Adapt<CreateAuthorCommand>();
            var response = await _mediator.Send(command, cancellationToken);
            response.RedirectUrl = Url.Action(nameof(GetById), new { id = response.Id });
            return Ok(response);
        }

        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateAuthorDTO updateAuthorDTO,
            CancellationToken cancellationToken)
        {
            var command = updateAuthorDTO.Adapt<UpdateAuthorCommand>();
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpDelete]
        [Route("{id:guid}/delete")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteAuthorCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
