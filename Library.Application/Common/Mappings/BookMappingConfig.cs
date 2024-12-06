﻿using Library.Application.BookUseCases.Commands;
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
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            config.NewConfig<UpdateBookCommand, Book>()
                .Map(dest => dest.ISBN, src => src.ISBN)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            config.NewConfig<Book, BookCatalogDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.GenreId, src => src.GenreId)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.ImageUrl);

            config.NewConfig<(Book Book, BookLending BookLending), BookLendingDTO>()
                .Map(dest => dest.Id, src => src.Book.Id)
                .Map(dest => dest.Title, src => src.Book.Title)
                .Map(dest => dest.Description, src => src.Book.Description)
                .Map(dest => dest.GenreId, src => src.Book.GenreId)
                .Map(dest => dest.AuthorId, src => src.Book.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.Book.ImageUrl)
                .Map(dest => dest.ReturnDate, src => src.BookLending.ReturnDate);

            config.NewConfig<(Book Book, bool IsAvalible), BookDetailsDTO>()
                .Map(dest => dest.Id, src => src.Book.Id)
                .Map(dest => dest.Title, src => src.Book.Title)
                .Map(dest => dest.Description, src => src.Book.Description)
                .Map(dest => dest.GenreId, src => src.Book.GenreId)
                .Map(dest => dest.AuthorId, src => src.Book.AuthorId)
                .Map(dest => dest.ImageUrl, src => src.Book.ImageUrl)
                .Map(dest => dest.ISBN, src => src.Book.ISBN)
                .Map(dest => dest.Author, src => src.Book.Author)
                .Map(dest => dest.Genre, src => src.Book.Genre)
                .Map(dest => dest.IsAvalible, src => src.IsAvalible);



            //DTOs => маппится в комманду;
            //по ID в хандлере получается исходная книга;
            //ненулевые свойства обновляются 
            //передается в репозиторий на обновление
        }
    }
}
