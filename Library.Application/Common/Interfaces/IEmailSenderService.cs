namespace Library.Application.Common.Interfaces
{
    public interface IEmailSenderService
    {
        Task<Dictionary<string, string>> GetUsersEmailsByIds(IEnumerable<string> userIds);
    }
}
