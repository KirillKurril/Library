using MediatR;
using Library.Domain.Entities;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record UpdateBookImageCommand(
        int BookId,
        string ImageUrl) : IRequest<Book>;
}
