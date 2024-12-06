using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Queries;
using Library.Application.Common.Exceptions;
using Library.Application.Common.Models;
using Library.Domain.Entities;
using Library.Presentation.Services.BookImage;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Library.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBookImageService _imageService;
        public BookController(IMediator mediator, IBookImageService bookImageService)
        {
            _mediator = mediator;
            _imageService = bookImageService;
        }

        [HttpGet]
        [Route("my-books")]
        public async Task<ActionResult<ResponseData<BookLendingDTO>>> GetBorrowedList(
            [FromQuery] int userId,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            try
            {
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
            [FromQuery] string? genre,
            [FromQuery] int? AuthorId,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            try
            {
                var query = new SearchBooksQuery(searchTerm, genre, AuthorId, pageNo, itemsPerPage);
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while searching books. {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<BookDetailsDTO>> GetById(
            int id,
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
        public async Task<IActionResult> BorrowBook(
            int id,
            [FromBody] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new BorrowBookCommand(id, userId);
                await _mediator.Send(command, cancellationToken);
                return NoContent();
            }
            catch (BookNotAvailableException ex)
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
        public async Task<IActionResult> ReturnBook(
            int id,
            [FromBody] int userId,
            CancellationToken cancellationToken)
        {
            try
            {
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
        public async  Task<ActionResult<CreateEntityResponse>> Create(
            [FromBody] CreateBookDTO createBookDTO,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = createBookDTO.Adapt<CreateBookCommand>();
                var response = await _mediator.Send(command, cancellationToken);
                response.RedirectUrl = Url.Action(nameof(GetById), new { id = response.Id});
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Errors);
            }
            catch (DuplicateIsbnException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while creating the book. {ex}");
            }
        }

        [HttpPut]
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
        public async Task<IActionResult> Delete(
            int id,
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

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage(
            int id,
            IFormFile image,
            CancellationToken cancellationToken)
        {
            try
            {
                string url = _imageService.SaveImage(image);
                var command = new UpdateBookImageCommand(id, url);
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
