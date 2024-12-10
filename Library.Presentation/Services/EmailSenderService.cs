using Library.Application.Common.Interfaces;
using Library.Application.Common.Models;

namespace Library.Presentation.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        public Task<ResponseData<bool>> SendNotifications(IReadOnlyList<DebtorNotification> notifications)
        {
            throw new NotImplementedException();
        }
    }
}
