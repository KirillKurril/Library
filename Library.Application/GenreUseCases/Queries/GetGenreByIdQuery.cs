namespace Library.Application.GenreUseCases.Queries
{
    public sealed record GetGenreByIdQuery(int Id) : IRequest<Genre>;
}
