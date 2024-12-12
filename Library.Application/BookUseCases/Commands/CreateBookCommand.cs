using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record CreateBookCommand(
        string ISBN,
        string Title,
        string? Description,
        int Quantity,
        Guid GenreId,
        Guid AuthorId) : IRequest<CreateEntityResponse>
    {
        public string? ImageUrl { get; set; }
    }
}
