using Library.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Library.Infrastructure.Services
{
    public class SmtpSettingsLocalFile : ISmtpSettings
    {
        private readonly IConfiguration _configuration;

        public SmtpSettingsLocalFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Server => _configuration.GetValue<string>("SmtpConfiguration:SmtpServer");
        public int Port => _configuration.GetValue<int>("SmtpConfiguration:Port");
        public string SenderEmail => _configuration.GetValue<string>("SmtpConfiguration:SenderEmail");
        public string Password => _configuration.GetValue<string>("SmtpConfiguration:SenderPassword");
    }
}
