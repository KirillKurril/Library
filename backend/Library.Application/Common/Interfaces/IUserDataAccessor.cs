using Library.Application.Common.Models;
using System.Text.Json;

namespace Library.Application.Common.Interfaces
{
    public interface IUserDataAccessor
    {
        Task<ResponseData<IEnumerable<DebtorNotification>>> EnrichNotifications(IEnumerable<DebtorNotification> notifications);
        Task<ResponseData<JsonElement>> GetUserDataAsJson(Guid userId);
        Task<bool> UserExist(Guid userId);
        bool IsAdmin();
        bool IsBookOwner(Guid userId);
    }
}
