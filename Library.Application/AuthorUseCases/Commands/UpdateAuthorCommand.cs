namespace Library.Application.AuthorUseCases.Commands
{
    public sealed record UpdateAuthorCommand(
        int Id,
        string Name,
        string Surname,
        DateTime BirthDate,
        string Country) : IRequest<Author>;
}
