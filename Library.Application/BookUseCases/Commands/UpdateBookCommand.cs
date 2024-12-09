using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record UpdateBookCommand(
        int Id,
        string ISBN,
        string Title,
        string? Description,
        int Quantity,
        int GenreId,
        int AuthorId,
        string? ImageUrl) : IRequest;
}
