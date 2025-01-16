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
                new Author() { Name = "Lev", Surname = "Tolstoy", BirthDate = new DateTime(1828, 9, 9), Country = "Russia" },
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

            var books = new List<Book>()
            {
                new Book() { Title = "1984", Author = authors[0], Description = "A dystopian novel about totalitarian regime", Genre = genres[2], ISBN = "978-0451524935", Quantity = 15 },
                new Book() { Title = "Animal Farm", Author = authors[0], Description = "An allegorical novella about Soviet Union", Genre = genres[0], ISBN = "978-0451526342", Quantity = 12 },
                new Book() { Title = "Homage to Catalonia", Author = authors[0], Description = "Memoir of Spanish Civil War", Genre = genres[1], ISBN = "978-0156421171", Quantity = 8 },

                new Book() { Title = "Pride and Prejudice", Author = authors[1], Description = "A romantic novel about prejudice and love", Genre = genres[5], ISBN = "978-0141439518", Quantity = 20 },
                new Book() { Title = "Emma", Author = authors[1], Description = "A novel about youthful hubris and romantic misunderstandings", Genre = genres[5], ISBN = "978-0141439587", Quantity = 15 },
                new Book() { Title = "Sense and Sensibility", Author = authors[1], Description = "A story of two sisters with different approaches to love", Genre = genres[5], ISBN = "978-0141439662", Quantity = 18 },

                new Book() { Title = "The Adventures of Tom Sawyer", Author = authors[2], Description = "Adventures of a young boy growing up along the Mississippi River", Genre = genres[0], ISBN = "978-0143039563", Quantity = 25 },
                new Book() { Title = "Adventures of Huckleberry Finn", Author = authors[2], Description = "A story about friendship and adventure on the Mississippi River", Genre = genres[0], ISBN = "978-0142437179", Quantity = 22 },
                new Book() { Title = "The Prince and the Pauper", Author = authors[2], Description = "A tale about two boys who switch places", Genre = genres[0], ISBN = "978-0143039204", Quantity = 14 },

                new Book() { Title = "The Great Gatsby", Author = authors[3], Description = "A story of the American Dream in the Roaring Twenties", Genre = genres[0], ISBN = "978-0743273565", Quantity = 30 },
                new Book() { Title = "Tender Is the Night", Author = authors[3], Description = "A story about the rise and fall of a young psychiatrist", Genre = genres[0], ISBN = "978-0684801544", Quantity = 16 },
                new Book() { Title = "This Side of Paradise", Author = authors[3], Description = "A story of post-WWI youth", Genre = genres[0], ISBN = "978-0486289991", Quantity = 12 },

                new Book() { Title = "The Old Man and the Sea", Author = authors[4], Description = "Story of an aging Cuban fisherman", Genre = genres[0], ISBN = "978-0684801223", Quantity = 25 },
                new Book() { Title = "A Farewell to Arms", Author = authors[4], Description = "A love story set during World War I", Genre = genres[0], ISBN = "978-0684801469", Quantity = 18 },
                new Book() { Title = "For Whom the Bell Tolls", Author = authors[4], Description = "A story set in the Spanish Civil War", Genre = genres[0], ISBN = "978-0684803356", Quantity = 20 },

                new Book() { Title = "War and Peace", Author = authors[5], Description = "Epic novel of Russian society during Napoleonic Era", Genre = genres[0], ISBN = "978-0140447934", Quantity = 15 },
                new Book() { Title = "Anna Karenina", Author = authors[5], Description = "Tragic story of married aristocrat and her love affair", Genre = genres[0], ISBN = "978-0143035008", Quantity = 18 },
                new Book() { Title = "The Death of Ivan Ilyich", Author = authors[5], Description = "Novella about death and the meaning of life", Genre = genres[0], ISBN = "978-0553210353", Quantity = 12 },

                new Book() { Title = "Great Expectations", Author = authors[6], Description = "Coming-of-age story of an orphan named Pip", Genre = genres[0], ISBN = "978-0141439563", Quantity = 20 },
                new Book() { Title = "A Tale of Two Cities", Author = authors[6], Description = "Historical novel set in London and Paris", Genre = genres[0], ISBN = "978-0141439600", Quantity = 16 },
                new Book() { Title = "Oliver Twist", Author = authors[6], Description = "Story of an orphan boy in London's criminal underworld", Genre = genres[0], ISBN = "978-0141439747", Quantity = 18 },

                new Book() { Title = "Mrs Dalloway", Author = authors[7], Description = "A day in the life of Clarissa Dalloway", Genre = genres[0], ISBN = "978-0156628709", Quantity = 14 },
                new Book() { Title = "To the Lighthouse", Author = authors[7], Description = "Story of the Ramsay family", Genre = genres[0], ISBN = "978-0156907392", Quantity = 12 },
                new Book() { Title = "Orlando: A Biography", Author = authors[7], Description = "Fantasy biography spanning several centuries", Genre = genres[0], ISBN = "978-0156701600", Quantity = 10 },

                new Book() { Title = "One Hundred Years of Solitude", Author = authors[8], Description = "Multi-generational story of the Buendía family", Genre = genres[0], ISBN = "978-0060883287", Quantity = 22 },
                new Book() { Title = "Love in the Time of Cholera", Author = authors[8], Description = "A love story spanning fifty years", Genre = genres[0], ISBN = "978-0307389732", Quantity = 18 },
                new Book() { Title = "Chronicle of a Death Foretold", Author = authors[8], Description = "Non-linear investigation of an honor killing", Genre = genres[0], ISBN = "978-1400034710", Quantity = 15 },

                new Book() { Title = "To Kill a Mockingbird", Author = authors[9], Description = "Story about racial injustice in the American South", Genre = genres[0], ISBN = "978-0446310789", Quantity = 30 },
                new Book() { Title = "Go Set a Watchman", Author = authors[9], Description = "Scout Finch returns to her hometown", Genre = genres[0], ISBN = "978-0062409866", Quantity = 20 },
                new Book() { Title = "The Mockingbird Next Door", Author = authors[9], Description = "A biography of Harper Lee by her neighbor", Genre = genres[1], ISBN = "978-0679602631", Quantity = 10 }
            };

            foreach (var author in authors)
                _unitOfWork.AuthorRepository.Add(author);

            foreach(var genre in genres)
                _unitOfWork.GenreRepository.Add(genre);

            foreach(var book in books)
                _unitOfWork.BookRepository.Add(book);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
