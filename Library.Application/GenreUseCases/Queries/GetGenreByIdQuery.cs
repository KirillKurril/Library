namespace Library.Application.GenreUseCases.Queries
{
    public sealed record GetGenreByIdQuery(Guid Id) : IRequest<Genre>;
}
