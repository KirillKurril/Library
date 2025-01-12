namespace Library.Application.BookUseCases.Commands
{
    public sealed record BorrowBookCommand(
        Guid BookId,
        Guid UserId) : IRequest;
}
