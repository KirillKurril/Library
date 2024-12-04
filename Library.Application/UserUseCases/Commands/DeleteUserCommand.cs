namespace Library.Application.UserUseCases.Commands
{
    public sealed record DeleteUserCommand(int Id) : IRequest<User>;
}
