using Library.Domain.Entities;
using Library.Persistance.Contexts;

namespace Library.Persistance.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly Lazy<IRepository<Book>> _bookRepository;
        private readonly Lazy<IRepository<BookLending>> _bookLendingRepository;
        private readonly Lazy<IRepository<Author>> _authorRepository;
        private readonly Lazy<IRepository<Genre>> _genreRepository;

        public EfUnitOfWork(AppDbContext context)
        {
            _context = context;

            _bookRepository = new Lazy<IRepository<Book>>(() =>
            new EfRepository<Book>(context));

            _bookLendingRepository = new Lazy<IRepository<BookLending>>(() =>
            new EfRepository<BookLending>(context));

            _authorRepository = new Lazy<IRepository<Author>>(() =>
             new EfRepository<Author>(context));

            _genreRepository = new Lazy<IRepository<Genre>>(() =>
             new EfRepository<Genre>(context));
        }
        public IRepository<Book> BookRepository
         => _bookRepository.Value;
        public IRepository<Author> AuthorRepository
         => _authorRepository.Value;
        public IRepository<Genre> GenreRepository
         => _genreRepository.Value;
        public IRepository<BookLending> BookLendingRepository
         => _bookLendingRepository.Value;
        public async Task SaveChangesAsync() =>
         await _context.SaveChangesAsync();
    }
}
