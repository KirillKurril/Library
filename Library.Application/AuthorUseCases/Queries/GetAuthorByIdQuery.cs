namespace Library.Application.AuthorUseCases.Queries
{
    public sealed record GetAuthorByIdQuery(int Id) : IRequest<Author>;
}
