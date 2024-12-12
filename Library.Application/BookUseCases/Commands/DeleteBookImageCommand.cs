namespace Library.Application.BookUseCases.Commands
{
    public sealed record DeleteBookImageCommand(Guid BookId) : IRequest;
}
