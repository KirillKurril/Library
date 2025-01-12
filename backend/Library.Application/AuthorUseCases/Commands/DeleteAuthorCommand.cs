namespace Library.Application.AuthorUseCases.Commands
{
    public sealed record DeleteAuthorCommand(Guid Id) : IRequest;
}
