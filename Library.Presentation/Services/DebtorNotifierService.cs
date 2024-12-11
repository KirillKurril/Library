using Library.Application.BookUseCases.Queries;
using Library.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Library.Presentation.Services
{
    public class DebtorNotifierService : IDebtorNotifierService, IHostedService, IDisposable
    {
        private Timer? _timer;
        private readonly IMediator _mediator;
        private readonly TimeSpan _taskInterval;
        private readonly IEmailSenderService _emailSender;
        private readonly IUserDataAccessor _userDataAccessor;
        private readonly ILogger<DebtorNotifierService> _logger;
        public DebtorNotifierService(
            IConfiguration configuration,
            IEmailSenderService emailSender,
            IMediator mediator,
            ILogger<DebtorNotifierService> logger,
            IUserDataAccessor userDataAccessor)
        {
            var reviewIntervalSettings = configuration
                .GetValue<int>("LibrarySettings:DebtorReviewIntervalInDays");
            _taskInterval = TimeSpan.FromDays(reviewIntervalSettings);
            _userDataAccessor = userDataAccessor;
            _emailSender = emailSender;
            _mediator = mediator;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan initialDelay = TimeSpan.FromSeconds(10);
            TimeSpan interval = TimeSpan.FromDays(7);

            _timer = new Timer(DoWork, null, initialDelay, _taskInterval);
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            Task.Run(async () =>
            {
                var notifications = await _mediator.Send(new GetExpiredBooksQuery());
                var notificationsEnrichResponse = await _userDataAccessor.EnrichNotifications(notifications);

                if (!notificationsEnrichResponse.Success)
                {
                    _logger.LogError(notificationsEnrichResponse.ErrorMessage);
                    return;
                }

                var sendNotificationsResponse = await _emailSender.SendNotifications(notifications);

                if (!sendNotificationsResponse.Success)
                {
                    _logger.LogError(sendNotificationsResponse.ErrorMessage);
                    return;
                }
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
