using Library.Domain.Entities;

namespace Library.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<Author> AuthorRepository { get; }
        IRepository<Book> BookRepository { get; }
        IRepository<Genre> GenreRepository { get; }
        Task SaveChangesAsync();
        Task DeleteDataBaseAsync();
        Task CreateDataBaseAsync();
    }
}
