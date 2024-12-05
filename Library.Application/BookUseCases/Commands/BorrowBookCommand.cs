using System;
using Library.Domain.Entities;
using MediatR;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record BorrowBookCommand(
        int BookId,
        int UserId) : IRequest<Book>;
}
