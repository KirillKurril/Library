using Library.Application.BookUseCases.Commands;
using Library.Application.BookUseCases.Queries;
using Library.Application.Common.Interfaces;
using Library.Application.Common.Models;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Presentation.Controllers
{
    /// <summary>
    /// Контроллер для работы с книгами
    /// </summary>
    [Route("books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBookImageService _imageService;
        private readonly IUserDataAccessor _userDataAccessor;
        private readonly ILogger<BookController> _logger;
        public BookController(
            IMediator mediator,
            IBookImageService bookImageService,
            IUserDataAccessor userDataAccessor,
            ILogger<BookController> logger)
        {
            _mediator = mediator;
            _imageService = bookImageService;
            _userDataAccessor = userDataAccessor;
            _logger = logger;
        }

        /// <summary>
        /// Получить список книг, взятых текущим пользователем
        /// </summary>
        /// <param name="pageNo">Номер страницы (опционально)</param>
        /// <param name="itemsPerPage">Количество элементов на странице (опционально)</param>
        /// <returns>Список взятых книг с пагинацией</returns>
        /// <response code="200">Список успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("/users/{userId:guid}/my-books")]
        [Authorize]
        [ProducesResponseType(typeof(PaginationListModel<IEnumerable<BookLendingDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginationListModel<IEnumerable<BookLendingDTO>>>> GetBorrowedList(

            [FromRoute] Guid userId,
            [FromQuery] string? searchTerm,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            if(_userDataAccessor.IsAdmin() || _userDataAccessor.IsBookOwner(userId))
            {
                var query = new GetBorrowedBooksQuery(userId, pageNo, itemsPerPage, searchTerm);
                var result = await _mediator.Send(query, cancellationToken);
                return Ok(result);
            }
            return Unauthorized();
        }

        /// <summary>
        /// Получить список книг по фильтру
        /// </summary>
        /// <param name="searchTerm">Строка поиска (опционально)</param>
        /// <param name="genreId">ID жанра (опционально)</param>
        /// <param name="AuthorId">ID автора (опционально)</param>
        /// <param name="pageNo">Номер страницы (опционально)</param>
        /// <param name="itemsPerPage">Количество элементов на странице (опционально)</param>
        /// <returns>Список книг с пагинацией</returns>
        /// <response code="200">Список успешно получен</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("catalog")]
        [ProducesResponseType(typeof(ResponseData<BookCatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PaginationListModel<BookCatalogDTO>>> GetFiltredList(
            [FromQuery] string? searchTerm,
            [FromQuery] Guid? genreId,
            [FromQuery] Guid? AuthorId,
            [FromQuery] int? pageNo,
            [FromQuery] int? itemsPerPage,
            CancellationToken cancellationToken)
        {
            var query = new SearchBooksQuery(searchTerm, genreId, AuthorId, pageNo, itemsPerPage);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Получить книгу по ID
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <returns>Книга</returns>
        /// <response code="200">Книга успешно получена</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("{id:guid}")]
        [ProducesResponseType(typeof(BookDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDetailsDTO>> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetBookByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Получить книгу по ISBN
        /// </summary>
        /// <param name="isbn">ISBN книги</param>
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
            [FromRoute] string isbn,
            CancellationToken cancellationToken)
        {
                var query = new GetBookByIsbnQuery(isbn);
                var result = await _mediator.Send(query, cancellationToken);

                return Ok(result);
        }

        /// <summary>
        /// Выдать книгу
        /// </summary>
        /// <param name="bookId">ID книги</param>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно взята</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="409">Книга уже взята</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("/users/{userId:guid}/books/{bookId:guid}/lend")]
        [AllowAnonymous]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LendBook(
            [FromRoute] Guid bookId,
            [FromRoute] Guid userId,
            CancellationToken cancellationToken)
        {
            var command = new LendBookCommand(bookId, userId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Вернуть книгу
        /// </summary>
        /// <param name="bookId">ID книги</param>
        /// <param name="userId">ID пользователя</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно возвращена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("/users/{userId:guid}/books/{bookId:guid}/return")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReturnBook(
            [FromRoute] Guid userId,
            [FromRoute] Guid bookId,
            CancellationToken cancellationToken)
        {
            var command = new ReturnBookCommand(bookId, userId);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Создать книгу
        /// </summary>
        /// <param name="createBookDTO">Данные для создания книги</param>
        /// <returns>Результат операции</returns>
        /// <response code="201">Книга успешно создана</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost]
        [Route("create")]
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
            if (result is NoContentResult)
                return Ok(createBookResult);
            else
                return StatusCode(500, $"Error setting default image to {createBookDTO.Title}");
        }

        /// <summary>
        /// Обновить книгу
        /// </summary>
        /// <param name="updateBookDTO">Данные для обновления книги</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно обновлена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="409">Книга уже существует</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPut]
        [Route("update")]
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
            var command = updateBookDTO.Adapt<UpdateBookCommand>();
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Удалить книгу
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Книга успешно удалена</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="409">Книга уже взята</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpDelete]
        [Route("{id:guid}/delete")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var command = new DeleteBookCommand(id);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Загрузить изображение книги
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <param name="image">Изображение обложки книги</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Изображение успешно загружено</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("{id:guid}/upload-cover/")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCover(
            [FromRoute] Guid id,
            IFormFile image,
            CancellationToken cancellationToken)
        {
            var urlResponse = await _imageService.SaveCoverImage(image, Request.Host, Request.Scheme);
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

        /// <summary>
        /// Установить изображение книги по умолчанию
        /// </summary>
        /// <param name="id">ID книги</param>
        /// <returns>Результат операции</returns>
        /// <response code="204">Изображение успешно установлено</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Книга не найдена</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpDelete("{id:guid}/remove-cover/")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetDefaultCover(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var urlResponse = _imageService.GetDefaultCoverURL(Request.Host, Request.Scheme);
            if (!urlResponse.Success)
            {
               return StatusCode(500, urlResponse.ErrorMessage);
            }
            var command = new UpdateBookImageCommand(id, urlResponse.Data);
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}
