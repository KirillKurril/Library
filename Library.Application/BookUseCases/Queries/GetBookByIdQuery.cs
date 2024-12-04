namespace Library.Application.BookUseCases.Queries
{
    public sealed record GetBookByIdQuery(int Id) : IRequest<Book>;
}
