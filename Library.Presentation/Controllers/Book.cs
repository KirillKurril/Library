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
    /// <summary>
    /// Контроллер для работы с книгами
    /// </summary>
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

        /// <summary>
        /// Получить список книг, взятых текущим пользователем
        /// </summary>
        /// <param name="pageNo">Номер страницы (опционально)</param>
        /// <param name="itemsPerPage">Количество элементов на странице (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список взятых книг с пагинацией</returns>
        /// <response code="200">Список успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("my-books")]
        [Authorize]
        [ProducesResponseType(typeof(PaginationListModel<IEnumerable<BookLendingDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Получить список книг по фильтру
        /// </summary>
        /// <param name="searchTerm">Строка поиска (опционально)</param>
        /// <param name="genreId">ID жанра (опционально)</param>
        /// <param name="AuthorId">ID автора (опционально)</param>
        /// <param name="pageNo">Номер страницы (опционально)</param>
        /// <param name="itemsPerPage">Количество элементов на странице (опционально)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список книг с пагинацией</returns>
        /// <response code="200">Список успешно получен</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("catalog")]
        [ProducesResponseType(typeof(ResponseData<BookCatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Получить книгу по ID
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Книга</returns>
        /// <response code="200">Книга успешно получена</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("{id:Guid}")]
        [ProducesResponseType(typeof(BookDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Получить книгу по ISBN
        /// </summary>
        /// <param name="isbn">ISBN книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Книга</returns>
        /// <response code="200">Книга успешно получена</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("{isbn}")]
        [ProducesResponseType(typeof(BookDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Взять книгу
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно взята</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="409">Книга уже взята</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("{id}/borrow")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Вернуть книгу
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно возвращена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("{id}/return")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Создать книгу
        /// </summary>
        /// <param name="createBookDTO">Данные для создания книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        /// <response code="201">Книга успешно создана</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(CreateEntityResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateEntityResponse>> Create(
            [FromBody] CreateBookDTO createBookDTO,
            CancellationToken cancellationToken)
        {
            var command = createBookDTO.Adapt<CreateBookCommand>();
            var createBookResult = await _mediator.Send(command, cancellationToken);
            createBookResult.RedirectUrl = Url.Action(nameof(GetById), new { id = createBookResult.Id });

            var result = await SetDefaultCover(createBookResult.Id, cancellationToken);
            if (result is OkResult)
                return Ok(createBookResult);

            else
                return StatusCode(500, $"Error setting default image to {createBookDTO.Title}");
        }

        /// <summary>
        /// Обновить книгу
        /// </summary>
        /// <param name="updateBookDTO">Данные для обновления книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно обновлена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="409">Книга уже существует</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPut]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Удалить книгу
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно удалена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="409">Книга уже взята</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpDelete]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Загрузить изображение книги
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <param name="image">Изображение книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Изображение успешно загружено</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("{id}/upload-image")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadImage(
            Guid id,
            IFormFile image,
            CancellationToken cancellationToken)
        {
            try
            {
                var urlResponse = await _imageService.SaveImage(image, Request.Host, Request.Scheme);
                if (!urlResponse.Success)
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

        /// <summary>
        /// Установить изображение книги по умолчанию
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Изображение успешно установлено</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("{id}/delete-cover")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
