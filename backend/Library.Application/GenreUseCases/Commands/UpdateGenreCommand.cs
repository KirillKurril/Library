namespace Library.Application.GenreUseCases.Commands
{
    public sealed record UpdateGenreCommand(Guid Id, string Name) : IRequest<Unit>;
}
