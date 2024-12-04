namespace Library.Application.AuthorUseCases.Commands
{
    public sealed record CreateAuthorCommand(
        string Name,
        string Surname,
        DateTime BirthDate,
        string Country) : IRequest<Author>;
}
