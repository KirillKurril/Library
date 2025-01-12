using Library.Application.DTOs;

namespace Library.Application.GenreUseCases.Commands
{
    public sealed record CreateGenreCommand(string Name) : IRequest<CreateEntityResponse>;
}
