namespace Library.Application.BookUseCases.Commands
{
    public sealed record UploadBookImageCommand(
        int BookId,
        string ImageUrl) : IRequest<Book>;
}
