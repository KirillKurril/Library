using Microsoft.Extensions.Configuration;
using Library.Application.Common.Interfaces;

namespace Library.Infrastructure.Services
{
    public class LibrarySettingsLocalFile : ILibrarySettings
    {
        private readonly IConfiguration _configuration;

        public LibrarySettingsLocalFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public double DefaultLoanPeriodInDays =>
            _configuration.GetValue<double>("LibrarySettings:DefaultLoanPeriodInDays");
        public double DebtorReviewIntervalInDays =>
            _configuration.GetValue<double>("LibrarySettings:DebtorReviewIntervalInDays");
        public string DefaulCoverFileName =>
            _configuration.GetValue<string>("LibrarySettings:DefaulCoverFileName");
    }
}
