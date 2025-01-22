using Library.Application.GenreUseCases.Commands;
using Library.Application.GenreUseCases.Queries;
using Library.Domain.Entities;
using MediatR;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Library.Application.Common.Models;

namespace Library.Presentation.Controllers
{
    [Route("genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IMediator _mediator;
        public GenreController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("filtred-list")]
        [ProducesResponseType(typeof(ResponseData<Author>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginationListModel<Genre>>> GetFiltredList(
            [FromQuery] string? searchTerm,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            var query = new GetGenresListQuery(searchTerm, pageNo, itemsPerPage);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("list")]
        public async Task<ActionResult<IEnumerable<Genre>>> GetAllList(
            CancellationToken cancellationToken)
        {
            var query = new GetAllGenresQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<Genre>> GetById(
           [FromRoute] Guid id,
           CancellationToken cancellationToken)
        {
            var command = new GetGenreByIdQuery(id);
            var genre = await _mediator.Send(command, cancellationToken);
            return Ok(genre);
        }


        [HttpPost]
        [Route("create")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<CreateEntityResponse>> Create(
            [FromBody] string genreName,
            CancellationToken cancellationToken)
        {
            var command = new CreateGenreCommand(genreName);
            var response = await _mediator.Send(command, cancellationToken);
            response.RedirectUrl = Url.Action(nameof(GetById), new { id = response.Id });
            return Ok(response);
        }

        [HttpPut]
        [Route("update")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateGenreDTO updateGenreDTO,
            CancellationToken cancellationToken)
        {
            var command = updateGenreDTO.Adapt<UpdateGenreCommand>();
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        [HttpDelete]
        [Route("{id:Guid}/delete")]
        //[Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(
                   [FromRoute] Guid id,
                   CancellationToken cancellationToken)
        {
            var command = new DeleteGenreCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
