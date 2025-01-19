using Library.Application.GenreUseCases.Commands;

namespace Library.Application.Common.Mappings
{
    public class GenreMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<string, Genre>()
                .Map(dest => dest.Name, src => src);

            config.NewConfig<CreateGenreCommand, Genre>()
                .Map(dest => dest.Name, src => src.Name);

            config.NewConfig<UpdateGenreCommand, Genre>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name);
        }
    }
}
