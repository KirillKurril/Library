using Microsoft.Extensions.Configuration;
using Library.Application.Common.Interfaces;

namespace Library.Persistance.Services
{
    public class LibrarySettingsService : ILibrarySettings
    {
        private readonly IConfiguration _configuration;

        public LibrarySettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int DefaultLoanPeriodInDays =>
            _configuration.GetValue<int>("LibrarySettings:DefaultLoanPeriodInDays");

        public int IsbnLength =>
            _configuration.GetValue<int>("LibrarySettings:DefaultIsbnLength");

    }
}
