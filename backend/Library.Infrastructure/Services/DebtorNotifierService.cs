using Library.Application.BookUseCases.Queries;
using Library.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Library.Presentation.Services
{
    public class DebtorNotifierService : IDebtorNotifierService, IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly TimeSpan _taskInterval;
        private readonly IEmailSenderService _emailSender;
        private readonly ILogger<DebtorNotifierService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public DebtorNotifierService(
            IConfiguration configuration,
            IEmailSenderService emailSender,
            ILogger<DebtorNotifierService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            var reviewIntervalSettings = configuration
                .GetValue<int>("LibrarySettings:DebtorReviewIntervalInDays");
            _taskInterval = TimeSpan.FromDays(reviewIntervalSettings);
            _emailSender = emailSender;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await SendDebtorNotificationsAsync(null);
            _timer = new Timer(SendDebtorNotifications, null, _taskInterval, _taskInterval);
            await Task.CompletedTask;
        }

        private void SendDebtorNotifications(object? state)
        {
            _ = SendDebtorNotificationsAsync(state).ContinueWith(task =>
            {
                if (task.IsFaulted && task.Exception != null)
                {
                    _logger.LogError(task.Exception, "An error occurred while processing debtor notifications");
                }
            });
        }

        private async Task SendDebtorNotificationsAsync(object? state)
        {
            if (!await _semaphore.WaitAsync(TimeSpan.Zero))
                return;

            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var userDataAccessor = scope.ServiceProvider.GetRequiredService<IUserDataAccessor>();

                var notifications = await mediator.Send(new GetExpiredBooksQuery());
                if(notifications.Count() == 0)
                {
                    _logger.LogInformation("No debtors were found");
                    return;
                }
                var notificationsEnrichResponse = await userDataAccessor.EnrichNotifications(notifications);

                if (!notificationsEnrichResponse.Success)
                {
                    _logger.LogError(notificationsEnrichResponse.ErrorMessage);
                    return;
                }

                var sendNotificationsResponse = await _emailSender.SendNotifications(notificationsEnrichResponse.Data);

                if (!sendNotificationsResponse.Success)
                {
                    _logger.LogError(sendNotificationsResponse.ErrorMessage);
                    return;
                }

                _logger.LogInformation($"Successfully sent {notifications.Count()} debtor notifications");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing debtor notifications");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _semaphore.Dispose();
        }
    }
}
