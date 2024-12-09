namespace Library.Presentation.Services;

public class OverdueBooksNotificationService : BackgroundService
{
    private readonly ILogger<OverdueBooksNotificationService> _logger;
    private readonly IKeycloakService _keycloakService;
    private readonly IConfiguration _configuration;
    // Inject your book loan repository here
    
    public OverdueBooksNotificationService(
        ILogger<OverdueBooksNotificationService> logger,
        IKeycloakService keycloakService,
        IConfiguration configuration)
    {
        _logger = logger;
        _keycloakService = keycloakService;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckOverdueBooks();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking overdue books");
            }

            // Wait for one week
            await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
        }
    }

    private async Task CheckOverdueBooks()
    {
        // Get overdue books from your repository
        var overdueLoans = new List<BookLoan>(); // Replace with actual repository call
        
        foreach (var loan in overdueLoans)
        {
            try
            {
                var userEmail = await _keycloakService.GetUserEmailById(loan.UserId);
                if (string.IsNullOrEmpty(userEmail))
                    continue;

                await SendOverdueNotification(userEmail, loan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process overdue loan {LoanId} for user {UserId}", 
                    loan.Id, loan.UserId);
            }
        }
    }

    private Task SendOverdueNotification(string email, BookLoan loan)
    {
        // Implement email sending logic here
        // You can use services like SendGrid, Amazon SES, or SMTP
        return Task.CompletedTask;
    }
}

public class BookLoan
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    // Add other properties
}
