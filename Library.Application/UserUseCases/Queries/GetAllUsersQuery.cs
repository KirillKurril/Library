namespace Library.Application.UserUseCases.Queries;

public sealed record GetAllUsersQuery : IRequest<IEnumerable<User>>;
