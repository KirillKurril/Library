namespace Library.Application.AuthorUseCases.Queries
{
    public sealed record GetAuthorByIdQuery(Guid Id) : IRequest<Author>;
}
