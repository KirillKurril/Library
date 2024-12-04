using System;
using Library.Domain.Entities;
using MediatR;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record UpdateBookCommand(
        int Id,
        string ISBN,
        string Title,
        string Description,
        Genre Genre,
        int AuthorId,
        string ImageUrl) : IRequest<Book>;
}
