namespace Library.Application.BookUseCases.Commands
{
    public sealed record ReturnBookCommand(Guid BookId, Guid UserId) : IRequest;
}
