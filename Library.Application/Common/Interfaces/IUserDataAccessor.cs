using Library.Application.Common.Models;
using System.Text.Json;

namespace Library.Application.Common.Interfaces
{
    public interface IUserDataAccessor
    {
        public Task<ResponseData<Dictionary<Guid, string>>> GetUsersEmailsByIds(IEnumerable<Guid> userIds);
        public Task<ResponseData<JsonElement>> GetUserDataAsJson(Guid userId);
        public Task<bool> UserExist(Guid userId);
    }
}
