namespace Library.Application.Common.Interfaces
{
    public interface IUserEmailAccessor
    {
        Task<Dictionary<string, string>> GetUsersEmailsByIds(IEnumerable<string> userIds);
    }
}
