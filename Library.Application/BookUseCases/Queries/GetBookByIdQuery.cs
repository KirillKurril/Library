using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBookByIdQuery(int Id) : IRequest<BookDetailsDTO>;
}
