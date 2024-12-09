using Library.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Persistance.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookLending> BookLendings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Book>()
                .Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Book>()
                .Property(b => b.ISBN)
                .IsRequired()
                .HasMaxLength(13);

            modelBuilder.Entity<Book>()
                .Property(b => b.Description)
                .HasMaxLength(2000);

            modelBuilder.Entity<Book>()
                .Property(b => b.Quantity)
                .IsRequired();

            modelBuilder.Entity<Book>()
                .Property(b => b.ImageUrl)
                .HasMaxLength(500);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany()
                .HasForeignKey(b => b.GenreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Author>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Author>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Author>()
                .Property(a => a.Surname)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Author>()
                .Property(a => a.Country)
                .HasMaxLength(100);

            modelBuilder.Entity<Genre>()
                .HasKey(g => g.Id);

            modelBuilder.Entity<Genre>()
                .Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<BookLending>()
                .HasKey(bl => bl.Id);

            modelBuilder.Entity<BookLending>()
                .HasOne<Book>()
                .WithMany()
                .HasForeignKey(bl => bl.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookLending>()
                .Property(bl => bl.UserId)
                .IsRequired();

            modelBuilder.Entity<BookLending>()
                .Property(bl => bl.BorrowedAt)
                .IsRequired();

            SeedData(modelBuilder);
        }
        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Фантастика" },
                new Genre { Id = 2, Name = "Детектив" },
                new Genre { Id = 3, Name = "Роман" },
                new Genre { Id = 4, Name = "Научпоп" }
            );

            modelBuilder.Entity<Author>().HasData(
                new Author
                {
                    Id = 1,
                    Name = "Лев",
                    Surname = "Толстой",
                    BirthDate = new DateTime(1828, 9, 9),
                    Country = "Россия"
                },
                new Author
                {
                    Id = 2,
                    Name = "Агата",
                    Surname = "Кристи",
                    BirthDate = new DateTime(1890, 9, 15),
                    Country = "Великобритания"
                },
                new Author
                {
                    Id = 3,
                    Name = "Стивен",
                    Surname = "Кинг",
                    BirthDate = new DateTime(1947, 9, 21),
                    Country = "США"
                }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "Война и мир",
                    ISBN = "978-5-389-01686-6",
                    AuthorId = 1,
                    GenreId = 3,
                    Quantity = 10,
                    Description = "Эпопея о жизни русского дворянства",
                    ImageUrl = "https://example.com/war-and-peace.jpg"
                },
                new Book
                {
                    Id = 2,
                    Title = "Убийство в Восточном экспрессе",
                    ISBN = "978-5-17-987654-3",
                    AuthorId = 2,
                    GenreId = 2,
                    Quantity = 5,
                    Description = "Детективный роман о расследовании убийства",
                    ImageUrl = "https://example.com/orient-express.jpg"
                },
                new Book
                {
                    Id = 3,
                    Title = "Сияние",
                    ISBN = "978-5-17-123456-7",
                    AuthorId = 3,
                    GenreId = 1,
                    Quantity = 7,
                    Description = "Психологический триллер о paranormal events",
                    ImageUrl = "https://example.com/shining.jpg"
                }
            );
        }
    }
}
