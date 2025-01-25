namespace Library.Application.Common.Interfaces
{
    public interface IDebtorNotifierService
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
