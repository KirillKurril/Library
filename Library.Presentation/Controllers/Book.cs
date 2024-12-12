using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Queries;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Models;
using Library.Presentation.Services.BookImage;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBookImageService _imageService;
        private readonly ILogger<BookController> _logger;
        public BookController(
            IMediator mediator,
            IBookImageService bookImageService,
            ILogger<BookController> logger)
        {
            _mediator = mediator;
            _imageService = bookImageService;
            _logger = logger;
        }

        [HttpGet]
        [Route("my-books")]
        [Authorize]
        public async Task<ActionResult<PaginationListModel<IEnumerable<BookLendingDTO>>>> GetBorrowedList(
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = new Guid(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                var query = new GetBorrowedBooksQuery(userId, pageNo, itemsPerPage);
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving borrowed books. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("catalog")]
        public async Task<ActionResult<ResponseData<BookCatalogDTO>>> GetFiltredList(
            [FromQuery] string? searchTerm,
            [FromQuery] Guid? genreId,
            [FromQuery] Guid? AuthorId,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new SearchBooksQuery(searchTerm, genreId, AuthorId, pageNo, itemsPerPage);
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while searching books. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<ActionResult<BookDetailsDTO>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetBookByIdQuery(id);
                var result = await _mediator.Send(query, cancellationToken);

                if (result == null)
                    return NotFound($"Book with ID {id} not found");

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving book with ID {id}. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{isbn:string}")]
        public async Task<ActionResult<BookDetailsDTO>> GetByISBN(
            string isbn,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetBookByIsbnQuery(isbn);
                var result = await _mediator.Send(query, cancellationToken);

                if (result == null)
                    return NotFound($"Book with ISBN {isbn} not found");

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, $"An error occurred while retrieving book with ISBN {isbn}");
            }
        }

        [HttpPost("{id}/borrow")]
        [Authorize]
        public async Task<IActionResult> BorrowBook(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = new Guid(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                var command = new BorrowBookCommand(id, userId);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (BookInUseException ex)
            {
                return Conflict(ex.Message);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, $"An error occurred while borrowing book with ID {id}");
            }
        }

        [HttpPost("{id}/return")]
        [Authorize]
        public async Task<IActionResult> ReturnBook(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var userId = new Guid(User.Claims.FirstOrDefault(c => c.Type == "id")?.Value);
                var command = new ReturnBookCommand(id, userId);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while returning book with ID {id}. {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<CreateEntityResponse>> Create(
            [FromBody] CreateBookDTO createBookDTO,
            CancellationToken cancellationToken)
        {
            var command = createBookDTO.Adapt<CreateBookCommand>();
            var createBookResult = await _mediator.Send(command, cancellationToken);
            createBookResult.RedirectUrl = Url.Action(nameof(GetById), new { id = createBookResult.Id});

            var result = await SetDefaultCover(createBookResult.Id, cancellationToken);
            if (result is OkResult)
                return Ok(createBookResult);

            else
                return StatusCode(500, $"Error setting default image to {createBookDTO.Title}");
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Update(
            [FromBody] UpdateBookDTO updateBookDTO,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = updateBookDTO.Adapt<UpdateBookCommand>();
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
            catch (DuplicateIsbnException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the book {ex.Message}");
            }
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new DeleteBookCommand(id);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (BookInUseException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting book with ID {id}. {ex.Message}");
            }
        }

        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UploadImage(
            Guid id,
            IFormFile image,
            CancellationToken cancellationToken)
        {
            try
            {
                var urlResponse = await _imageService.SaveImage(image, Request.Host, Request.Scheme);
                if(!urlResponse.Success)
                {
                    _logger.LogError(urlResponse.ErrorMessage);
                    if (urlResponse.ErrorMessage.Contains("is not an image"))
                        return BadRequest(urlResponse.ErrorMessage);
                    else
                        return StatusCode(500, urlResponse.ErrorMessage);
                }
                var command = new UpdateBookImageCommand(id, urlResponse.Data);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while uploading image for book with ID {id}. {ex.Message}");
            }
        }

        [HttpPost("{id}/delete-cover")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> SetDefaultCover(
            Guid id,
            CancellationToken cancellationToken)
        {
            try
            {
                var urlResponse = _imageService.GetDefaultCoverImage(Request.Host, Request.Scheme);
                if (!urlResponse.Success)
                {
                    return StatusCode(500, urlResponse.ErrorMessage);
                }
                var command = new UpdateBookImageCommand(id, urlResponse.Data);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while uploading image for book with ID {id}. {ex.Message}");
            }
        }
    }
}
