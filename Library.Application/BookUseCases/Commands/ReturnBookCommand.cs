using System;
using Library.Domain.Entities;
using MediatR;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record ReturnBookCommand(int BookId, int UserId) : IRequest;
}
