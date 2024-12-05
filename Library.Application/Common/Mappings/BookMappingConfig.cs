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

            //DTOs => маппится в комманду;
            //по ID в хандлере получается исходная книга;
            //ненулевые свойства обновляются 
            //передается в репозиторий на обновление
        }
    }
}
