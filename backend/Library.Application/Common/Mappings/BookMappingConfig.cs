using Library.Application.BookUseCases.Commands;
using Library.Application.DTOs;

namespace Library.Application.Common.Mappings
{
    public class BookMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateBookDTO, CreateBookCommand>()
                .Map(dest => dest.ISBN, src => src.ISBN)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId);

            config.NewConfig<CreateBookCommand, Book>()
                .Map(dest => dest.ISBN, src => src.ISBN)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId);

            config.NewConfig<UpdateBookCommand, Book>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.ISBN, src => src.ISBN)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            config.NewConfig<JoinLendingDTO, BookLendingDTO>()
                .Map(dest => dest.Id, src => src.Book.Id)
                .Map(dest => dest.Title, src => src.Book.Title)
                .Map(dest => dest.Description, src => src.Book.Description)
                .Map(dest => dest.GenreId, src => src.Book.GenreId)
                .Map(dest => dest.AuthorId, src => src.Book.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.Book.ImageUrl)
                .Map(dest => dest.ReturnDate, src => src.BookLending.ReturnDate);

            config.NewConfig<Book, BookCatalogDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            config.NewConfig<Book, BookDetailsDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl)
                .Map(dest => dest.ISBN, src => src.ISBN)
                .Map(dest => dest.Author, src => src.Author)
                .Map(dest => dest.Genre, src => src.Genre)
                .Map(dest => dest.IsAvailable, src => src.IsAvailable);
        }
    }
}
