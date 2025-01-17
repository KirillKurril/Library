using Library.Application.Common.Interfaces;
using Library.Domain.Entities;

namespace Library.Persistance.Services
{
    public class DbInitializer : IDbInitializer
    {
        IUnitOfWork _unitOfWork;
        public DbInitializer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Seed()
        {
            var authors = new List<Author>()
            {
                new Author() { Name = "George", Surname = "Orwell", BirthDate = new DateTime(1903, 6, 25), Country = "United Kingdom" },
                new Author() { Name = "Jane", Surname = "Austen", BirthDate = new DateTime(1775, 12, 16), Country = "United Kingdom" },
                new Author() { Name = "Mark", Surname = "Twain", BirthDate = new DateTime(1835, 11, 30), Country = "United States" },
                new Author() { Name = "F. Scott", Surname = "Fitzgerald", BirthDate = new DateTime(1896, 9, 24), Country = "United States" },
                new Author() { Name = "Ernest", Surname = "Hemingway", BirthDate = new DateTime(1899, 7, 21), Country = "United States" },
                new Author() { Name = "Leo", Surname = "Tolstoy", BirthDate = new DateTime(1828, 9, 9), Country = "Russia" },
                new Author() { Name = "Charles", Surname = "Dickens", BirthDate = new DateTime(1812, 2, 7), Country = "United Kingdom" },
                new Author() { Name = "Virginia", Surname = "Woolf", BirthDate = new DateTime(1882, 1, 25), Country = "United Kingdom" },
                new Author() { Name = "Gabriel", Surname = "Garcia Marquez", BirthDate = new DateTime(1927, 3, 6), Country = "Colombia" },
                new Author() { Name = "Harper", Surname = "Lee", BirthDate = new DateTime(1926, 4, 28), Country = "United States" }
            };

            var genres = new List<Genre>()
            {
                new Genre() { Name = "Fiction" },
                new Genre() { Name = "Non-Fiction" },
                new Genre() { Name = "Science Fiction" },
                new Genre() { Name = "Fantasy" },
                new Genre() { Name = "Mystery" },
                new Genre() { Name = "Romance" }
            };

            foreach (var author in authors)
                _unitOfWork.AuthorRepository.Add(author);
            
            foreach(var genre in genres)
                _unitOfWork.GenreRepository.Add(genre);

            try 
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving authors and genres: {ex.Message}", ex);
            }

            var books = new List<Book>()
            {
                new Book() { Title = "1984", AuthorId = authors[0].Id, Description = "A dystopian novel about totalitarian regime", GenreId = genres[2].Id, ISBN = "978-0451524935", Quantity = 15, ImageUrl="https://localhost:7020/images/covers/1984.jpg"},
                new Book() { Title = "Animal Farm", AuthorId = authors[0].Id, Description = "An allegorical novella about Soviet Union", GenreId = genres[0].Id, ISBN = "978-0451526342", Quantity = 12, ImageUrl="https://localhost:7020/images/covers/Animal Farm.jpg" },
                new Book() { Title = "Homage to Catalonia", AuthorId = authors[0].Id, Description = "Memoir of Spanish Civil War", GenreId = genres[1].Id, ISBN = "978-0156421171", Quantity = 8, ImageUrl="https://localhost:7020/images/covers/Homage to Catalonia.jpg" },

                new Book() { Title = "Pride and Prejudice", AuthorId = authors[1].Id, Description = "A romantic novel about prejudice and love", GenreId = genres[5].Id, ISBN = "978-0141439518", Quantity = 20, ImageUrl="https://localhost:7020/images/covers/Pride and Prejudice.jpg" },
                new Book() { Title = "Emma", AuthorId = authors[1].Id, Description = "A novel about youthful hubris and romantic misunderstandings", GenreId = genres[5].Id, ISBN = "978-0141439587", Quantity = 15, ImageUrl="https://localhost:7020/images/covers/Emma.jpg" },
                new Book() { Title = "Sense and Sensibility", AuthorId = authors[1].Id, Description = "A story of two sisters with different approaches to love", GenreId = genres[5].Id, ISBN = "978-0141439662", Quantity = 18, ImageUrl="https://localhost:7020/images/covers/Sense and Sensibility.jpg" },

                new Book() { Title = "The Adventures of Tom Sawyer", AuthorId = authors[2].Id, Description = "Adventures of a young boy growing up along the Mississippi River", GenreId = genres[0].Id, ISBN = "978-0143039563", Quantity = 25, ImageUrl="https://localhost:7020/images/covers/The Adventures of Tom Sawyer.jpg" },
                new Book() { Title = "Adventures of Huckleberry Finn", AuthorId = authors[2].Id, Description = "A story about friendship and adventure on the Mississippi River", GenreId = genres[0].Id, ISBN = "978-0142437179", Quantity = 22, ImageUrl="https://localhost:7020/images/covers/Adventures of Huckleberry Finn.jpg" },
                new Book() { Title = "The Prince and the Pauper", AuthorId = authors[2].Id, Description = "A tale about two boys who switch places", GenreId = genres[0].Id, ISBN = "978-0143039204", Quantity = 14, ImageUrl="https://localhost:7020/images/covers/The Prince and the Pauper.jpg" },

                new Book() { Title = "The Great Gatsby", AuthorId = authors[3].Id, Description = "A story of the American Dream in the Roaring Twenties", GenreId = genres[0].Id, ISBN = "978-0743273565", Quantity = 30, ImageUrl="https://localhost:7020/images/covers/The Great Gatsby.jpg" },
                new Book() { Title = "Tender Is the Night", AuthorId = authors[3].Id, Description = "A story about the rise and fall of a young psychiatrist", GenreId = genres[0].Id, ISBN = "978-0684801544", Quantity = 16, ImageUrl="https://localhost:7020/images/covers/Tender Is the Night.jpg" },
                new Book() { Title = "This Side of Paradise", AuthorId = authors[3].Id, Description = "A story of post-WWI youth", GenreId = genres[0].Id, ISBN = "978-0486289991", Quantity = 12, ImageUrl="https://localhost:7020/images/covers/This Side of Paradise.jpg" },

                new Book() { Title = "The Old Man and the Sea", AuthorId = authors[4].Id, Description = "Story of an aging Cuban fisherman", GenreId = genres[0].Id, ISBN = "978-0684801223", Quantity = 25, ImageUrl="https://localhost:7020/images/covers/The Old Man and the Sea.jpg" },
                new Book() { Title = "A Farewell to Arms", AuthorId = authors[4].Id, Description = "A love story set during World War I", GenreId = genres[0].Id, ISBN = "978-0684801469", Quantity = 18, ImageUrl="https://localhost:7020/images/covers/A Farewell to Arms.jpg" },
                new Book() { Title = "For Whom the Bell Tolls", AuthorId = authors[4].Id, Description = "A story set in the Spanish Civil War", GenreId = genres[0].Id, ISBN = "978-0684803356", Quantity = 20, ImageUrl="https://localhost:7020/images/covers/For Whom the Bell Tolls.jpg" },

                new Book() { Title = "War and Peace", AuthorId = authors[5].Id, Description = "Epic novel of Russian society during Napoleonic Era", GenreId = genres[0].Id, ISBN = "978-0140447934", Quantity = 15, ImageUrl="https://localhost:7020/images/covers/War and Peace.jpg" },
                new Book() { Title = "Anna Karenina", AuthorId = authors[5].Id, Description = "Tragic story of married aristocrat and her love affair", GenreId = genres[0].Id, ISBN = "978-0143035008", Quantity = 18, ImageUrl="https://localhost:7020/images/covers/Anna Karenina.jpg" },
                new Book() { Title = "The Death of Ivan Ilyich", AuthorId = authors[5].Id, Description = "Novella about death and the meaning of life", GenreId = genres[0].Id, ISBN = "978-0553210353", Quantity = 12, ImageUrl="https://localhost:7020/images/covers/The Death of Ivan Ilyich.jpg" },

                new Book() { Title = "Great Expectations", AuthorId = authors[6].Id, Description = "Coming-of-age story of an orphan named Pip", GenreId = genres[0].Id, ISBN = "978-0141439563", Quantity = 20, ImageUrl="https://localhost:7020/images/covers/Great Expectations.jpg" },
                new Book() { Title = "A Tale of Two Cities", AuthorId = authors[6].Id, Description = "Historical novel set in London and Paris", GenreId = genres[0].Id, ISBN = "978-0141439600", Quantity = 16, ImageUrl="https://localhost:7020/images/covers/A Tale of Two Cities.jpg" },
                new Book() { Title = "Oliver Twist", AuthorId = authors[6].Id, Description = "Story of an orphan boy in London's criminal underworld", GenreId = genres[0].Id, ISBN = "978-0141439747", Quantity = 18, ImageUrl="https://localhost:7020/images/covers/Oliver Twist.jpg" },

                new Book() { Title = "Mrs Dalloway", AuthorId = authors[7].Id, Description = "A day in the life of Clarissa Dalloway", GenreId = genres[0].Id, ISBN = "978-0156628709", Quantity = 14, ImageUrl="https://localhost:7020/images/covers/Mrs Dalloway.jpg" },
                new Book() { Title = "To the Lighthouse", AuthorId = authors[7].Id, Description = "Story of the Ramsay family", GenreId = genres[0].Id, ISBN = "978-0156907392", Quantity = 12, ImageUrl="https://localhost:7020/images/covers/To the Lighthouse.jpg" },
                new Book() { Title = "Orlando: A Biography", AuthorId = authors[7].Id, Description = "Fantasy biography spanning several centuries", GenreId = genres[0].Id, ISBN = "978-0156701600", Quantity = 10, ImageUrl="https://localhost:7020/images/covers/Orlando A Biography.jpg" },

                new Book() { Title = "One Hundred Years of Solitude", AuthorId = authors[8].Id, Description = "Multi-generational story of the Buendía family", GenreId = genres[0].Id, ISBN = "978-0060883287", Quantity = 22, ImageUrl="https://localhost:7020/images/covers/One Hundred Years of Solitude.jpg" },
                new Book() { Title = "Love in the Time of Cholera", AuthorId = authors[8].Id, Description = "A love story spanning fifty years", GenreId = genres[0].Id, ISBN = "978-0307389732", Quantity = 18, ImageUrl="https://localhost:7020/images/covers/Love in the Time of Cholera.jpg" },
                new Book() { Title = "Chronicle of a Death Foretold", AuthorId = authors[8].Id, Description = "Non-linear investigation of an honor killing", GenreId = genres[0].Id, ISBN = "978-1400034710", Quantity = 15, ImageUrl="https://localhost:7020/images/covers/Chronicle of a Death Foretold.jpg" },

                new Book() { Title = "To Kill a Mockingbird", AuthorId = authors[9].Id, Description = "Story about racial injustice in the American South", GenreId = genres[0].Id, ISBN = "978-0446310789", Quantity = 30, ImageUrl="https://localhost:7020/images/covers/To Kill a Mockingbird.jpg" },
                new Book() { Title = "Go Set a Watchman", AuthorId = authors[9].Id, Description = "Scout Finch returns to her hometown", GenreId = genres[0].Id, ISBN = "978-0062409866", Quantity = 20, ImageUrl="https://localhost:7020/images/covers/Go Set a Watchman.jpg" },
                new Book() { Title = "The Mockingbird Next Door", AuthorId = authors[9].Id, Description = "A biography of Harper Lee by her neighbor", GenreId = genres[1].Id, ISBN = "978-0679602631", Quantity = 10, ImageUrl="https://localhost:7020/images/covers/The Mockingbird Next Door.jpg" }
            };

            foreach(var book in books)
                _unitOfWork.BookRepository.Add(book);

            try 
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving books: {ex.Message}", ex);
            }
        }
    }
}
