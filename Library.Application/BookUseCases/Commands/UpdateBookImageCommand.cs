using MediatR;
using Library.Domain.Entities;
using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record UpdateBookImageCommand(
        int BookId,
        string ImageUrl) : IRequest;
}
