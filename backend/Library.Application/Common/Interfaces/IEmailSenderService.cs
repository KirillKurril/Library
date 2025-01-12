using Library.Application.Common.Models;

namespace Library.Application.Common.Interfaces
{
    public interface IEmailSenderService
    {
        Task<ResponseData<bool>> SendNotifications(IEnumerable<DebtorNotification> notifications);
    }
}
