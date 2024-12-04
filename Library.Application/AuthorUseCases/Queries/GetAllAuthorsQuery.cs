namespace Library.Application.AuthorUseCases.Queries
{
    public sealed record GetAllAuthorsQuery() : IRequest<IEnumerable<Author>>;
}
