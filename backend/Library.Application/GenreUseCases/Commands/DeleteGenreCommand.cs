namespace Library.Application.GenreUseCases.Commands
{
    public sealed record DeleteGenreCommand(Guid Id) : IRequest<Unit>;
}
