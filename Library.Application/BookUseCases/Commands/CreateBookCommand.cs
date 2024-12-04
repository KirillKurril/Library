namespace Library.Application.BookUseCases.Commands
{
    public sealed record CreateBookCommand(
        string ISBN,
        string Title,
        string Description,
        Genre Genre,
        int AuthorId,
        string ImageUrl) : IRequest<Book>;
}
