namespace Library.Application.AuthorUseCases.Commands
{
    public sealed record UpdateAuthorCommand(
        Guid Id,
        string Name,    
        string Surname,
        DateTime? BirthDate,
        string? Country) : IRequest<Author>;
}
