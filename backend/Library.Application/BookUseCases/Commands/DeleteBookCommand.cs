namespace Library.Application.BookUseCases.Commands
{
    public sealed record DeleteBookCommand(Guid Id) : IRequest<Unit>;
}
