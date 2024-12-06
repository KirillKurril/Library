using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBookByIsbnQuery(string ISBN) : IRequest<BookDetailsDTO>;
}
