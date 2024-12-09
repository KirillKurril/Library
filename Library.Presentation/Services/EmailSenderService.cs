using Library.Application.Common.Interfaces;

namespace Library.Presentation.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public Task<Dictionary<string, string>> GetUsersEmailsByIds(IEnumerable<string> userIds)
        {
            throw new NotImplementedException();
        }
    }
}
