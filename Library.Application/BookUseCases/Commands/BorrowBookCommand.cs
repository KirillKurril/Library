namespace Library.Application.BookUseCases.Commands
{
    public sealed record BorrowBookCommand(
        int BookId,
        Guid UserId) : IRequest;
}
