namespace Library.Application.BookUseCases.Commands
{
    public sealed record UpdateBookCommand(
        Guid Id,
        string? ISBN,
        string? Title,
        string? Description,
        int? Quantity,
        Guid? GenreId,
        Guid? AuthorId,
        string? ImageUrl) : IRequest;
}
