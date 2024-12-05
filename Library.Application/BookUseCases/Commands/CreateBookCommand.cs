namespace Library.Application.BookUseCases.Commands
{
    public sealed record CreateBookCommand(
        string ISBN,
        string Title,
        string? Description,
        int Quantity,
        int GenreId,
        int AuthorId) : IRequest<Book>
    {
        public string? ImageUrl { get; set; }
    }
}
