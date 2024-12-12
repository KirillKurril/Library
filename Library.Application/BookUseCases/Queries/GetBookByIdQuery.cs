using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBookByIdQuery(Guid Id) : IRequest<BookDetailsDTO>;
}
