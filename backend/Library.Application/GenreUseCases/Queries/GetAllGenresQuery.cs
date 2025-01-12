namespace Library.Application.GenreUseCases.Queries
{
    public sealed record GetAllGenresQuery : IRequest<IEnumerable<Genre>>;
}
