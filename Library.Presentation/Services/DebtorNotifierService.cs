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
                var debtorIds = notifications.Select(n => n.UserID).ToList();
                var emailsDictionaryResponse = await _userDataAccessor.GetUsersEmailsByIds(debtorIds);
                
                if(!emailsDictionaryResponse.Success)
                {
                    _logger.LogError(emailsDictionaryResponse.ErrorMessage);
                    return;
                }

                foreach (var notification in notifications)
                {
                    notification.Email = emailsDictionaryResponse.Data[notification.UserID];
                }

                _emailSender.
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
