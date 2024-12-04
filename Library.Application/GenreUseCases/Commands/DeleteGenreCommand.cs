namespace Library.Application.GenreUseCases.Commands
{
    public sealed record DeleteGenreCommand(int Id) : IRequest<Genre>;
}
