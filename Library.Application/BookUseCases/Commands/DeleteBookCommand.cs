namespace Library.Application.BookUseCases.Commands
{
    public sealed record DeleteBookCommand(int Id) : IRequest<Book>;
}
