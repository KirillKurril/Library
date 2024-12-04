namespace Library.Application.AuthorUseCases.Commands
{
    public sealed record DeleteAuthorCommand(int Id) : IRequest<Author>;
}
