using Library.Application.Common.Interfaces;
using Library.Application.Common.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Library.Presentation.Services
{
    public class EmailSenderService : IEmailSenderService 
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _senderEmail;
        private readonly string _senderPassword;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(
            IConfiguration configuration,
            ILogger<EmailSenderService> logger)
        {
            var smtpConfiguration = configuration.GetRequiredSection("SmtpConfiguration");
            _smtpServer = smtpConfiguration.GetValue<string>("SmtpServer");
            _port = smtpConfiguration.GetValue<int>("Port");
            _senderEmail = smtpConfiguration.GetValue<string>("SenderEmail");
            _senderPassword = smtpConfiguration.GetValue<string>("SenderPassword");

            _logger = logger;
        }

        public async Task<ResponseData<bool>> SendNotifications(IEnumerable<DebtorNotification> notificaions)
        {
            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(_smtpServer, _port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_senderEmail, _senderPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying connect smpt server {_smtpServer}:\n{ex.Message}");
                await client.DisconnectAsync(true);
                client.Dispose();
                return new ResponseData<bool>(false, ex.Message);
            }
            var tasks = new List<Task>();

            foreach (var notification in notificaions)
            {
                try
                {
                    tasks.Add(Task.Run(async () => {
                        var letter = CreateEmailMessage(notification);
                        await client.SendAsync(letter);
                    }));

                    _logger.LogInformation($"Email sent to {notification.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sent email to {notification.Email}:\n{ex.Message}");
                }
            }

            await client.DisconnectAsync(true);
            client.Dispose();

            return new ResponseData<bool>(true);
        }

        private MimeMessage CreateEmailMessage(DebtorNotification notification)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Library Admin", _senderEmail));
            emailMessage.To.Add(new MailboxAddress(notification.FirstName + " " + notification.LastName, notification.Email));
            emailMessage.Subject = "Reminder: Books Not Returned on Time";

            var booksList = string.Join("<li>", notification.ExpiredBooks.Select(book =>
                $"<strong>{book.BookName}</strong> by {book.AuthorName}</li>"));
              
            emailMessage.Body = new TextPart("html")
            {
                Text = $@"
            <html>
            <body>
                <h1>Dear {notification.FirstName} {notification.LastName},</h1>
                <p>We hope you're doing well!</p>
                <p>This is a friendly reminder that the following books you borrowed from our library have not yet been returned:</p>
                <ul>
                    {booksList}
                </ul>
                <p>Please return these books as soon as possible.</p>
                <p>If you have any questions, feel free to contact us at any time.</p>
                <p>Thank you for being a valued member of our library!</p>
                <p>Best regards,<br/>The Library Team</p>
            </body>
            </html>
        "
            };

            return emailMessage;
        }

    }
}
