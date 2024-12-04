namespace Library.Application.BookUseCases.Commands
{
    public sealed record DeleteBookImageCommand(int BookId) : IRequest<Book>;
}
