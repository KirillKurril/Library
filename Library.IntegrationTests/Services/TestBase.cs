using Library.Domain.Entities;
using Library.Persistance.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

public abstract class TestBase : IDisposable
{
    protected AppDbContext _appDbContext;
    private readonly string _dbName;

    protected TestBase()
    {
        _dbName = $"TestDb_{Guid.NewGuid()}";
    }

    protected AppDbContext CreateContext()
    {
        // Сначала подключаемся к master для создания новой БД
        var masterConnection = new SqlConnection(
            "Server=(localdb)\\mssqllocaldb;" +
            "Database=master;" +
            "Trusted_Connection=True;" +
            "TrustServerCertificate=True"
        );

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(masterConnection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            // Удаляем БД если она существует и создаем новую
            context.Database.ExecuteSqlRaw($@"
                IF EXISTS (SELECT * FROM sys.databases WHERE name = '{_dbName}')
                BEGIN
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];
                END
                CREATE DATABASE [{_dbName}]");
        }

        // Теперь подключаемся к созданной БД
        var connectionString = 
            $"Server=(localdb)\\mssqllocaldb;" +
            $"Database={_dbName};" +
            "Trusted_Connection=True;" +
            "TrustServerCertificate=True";

        options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        _appDbContext = new AppDbContext(options);
        _appDbContext.Database.EnsureCreated();

        return _appDbContext;
    }

    protected (Author author, Genre genre) CreateTestEntities(AppDbContext context)
    {
        var author = new Author { Name = "Test Author" };
        var genre = new Genre { Name = "Test Genre" };
        
        author = context.Authors.Add(author).Entity;
        genre = context.Genres.Add(genre).Entity;
        context.SaveChanges();

        return (author, genre);
    }

    protected Book CreateTestBook(AppDbContext context, Author author, Genre genre, string title = "Test Book", string isbn = "1234567890123")
    {
        var book = new Book
        {
            Title = title,
            ISBN = isbn,
            AuthorId = author.Id,
            GenreId = genre.Id,
            Quantity = 1
        };

        book = context.Books.Add(book).Entity;
        context.SaveChanges();

        return book;
    }

    public void Dispose()
    {
        if (_appDbContext != null)
        {
            _appDbContext.Dispose();

            // Подключаемся к master для удаления тестовой БД
            var masterConnection = new SqlConnection(
                "Server=(localdb)\\mssqllocaldb;" +
                "Database=master;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True"
            );

            using var context = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(masterConnection)
                    .Options);

            // Принудительно закрываем все подключения и удаляем БД
            context.Database.ExecuteSqlRaw($@"
                IF EXISTS (SELECT * FROM sys.databases WHERE name = '{_dbName}')
                BEGIN
                    ALTER DATABASE [{_dbName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                    DROP DATABASE [{_dbName}];
                END");
        }
    }
}