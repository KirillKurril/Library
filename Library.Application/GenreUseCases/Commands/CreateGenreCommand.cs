namespace Library.Application.GenreUseCases.Commands
{
    public sealed record CreateGenreCommand(string Name) : IRequest<Genre>;
}
