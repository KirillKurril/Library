namespace Library.Application.BookUseCases.Commands
{
    public sealed record UpdateBookImageCommand(
        Guid BookId,
        string ImageUrl) : IRequest;
}
