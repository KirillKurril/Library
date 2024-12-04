namespace Library.Application.UserUseCases.Queries;

public sealed record GetUserByIdQuery(int Id) : IRequest<User>;
