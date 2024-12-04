using Library.Application.AuthorUseCases.Commands;

namespace Library.Application.Common.Mappings;

public class AuthorMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateAuthorCommand, Author>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Surname, src => src.Surname)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.Country, src => src.Country);

        config.NewConfig<UpdateAuthorCommand, Author>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Surname, src => src.Surname)
            .Map(dest => dest.BirthDate, src => src.BirthDate)
            .Map(dest => dest.Country, src => src.Country);
    }
}
