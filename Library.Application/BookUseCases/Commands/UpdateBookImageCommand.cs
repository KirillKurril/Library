using MediatR;
using Library.Domain.Entities;
using Library.Application.DTOs;

namespace Library.Application.BookUseCases.Commands
{
    public sealed record UpdateBookImageCommand(
        Guid BookId,
        string ImageUrl) : IRequest;
}
