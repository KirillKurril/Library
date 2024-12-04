namespace Library.Application.UserUseCases.Commands
{
    public sealed record CreateUserCommand(
        string UserName,
        string Email,
        string FirstName,
        string LastName) : IRequest<User>;
}
