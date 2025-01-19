namespace Library.Application.BookUseCases.Commands
{
    public sealed record LendBookCommand(
        Guid BookId,
        Guid UserId) : IRequest;
}
