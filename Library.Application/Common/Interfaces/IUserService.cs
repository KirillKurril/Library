namespace Library.Application.Common.Interfaces
{
    public interface IUserService
    {
        Task<bool> UserExistsAsync(int userId);
    }
}
