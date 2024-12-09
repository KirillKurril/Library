using Library.Application.DTOs;

namespace Library.Application.AuthorUseCases.Queries
{
    public sealed record GetAllForFiltrationAuthorsQuery() : IRequest<IEnumerable<AuthorBriefDTO>>;
}
