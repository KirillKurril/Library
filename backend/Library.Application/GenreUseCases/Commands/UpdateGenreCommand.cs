namespace Library.Application.GenreUseCases.Commands
{
    public sealed record UpdateGenreCommand(Guid id, string genreName) : IRequest;
}
