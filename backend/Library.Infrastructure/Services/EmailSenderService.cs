﻿using Library.Application.Common.Interfaces;
using Library.Application.Common.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace Library.Infrastructure.Services
{
    public class EmailSenderService : IEmailSenderService 
    {
        private readonly ISmtpSettings _settings;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(
            ISmtpSettings settings,
            ILogger<EmailSenderService> logger)
        {
            _settings = settings;
            _logger = logger;
        }

        public async Task<ResponseData<bool>> SendNotifications(IEnumerable<DebtorNotification> notificaions)
        {
            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(
                    _settings.Server,
                    _settings.Port,
                    SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(
                    _settings.SenderEmail,
                    _settings.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error trying connect smpt server {_settings.Server}:{_settings.Port}\n{ex.Message}");
                await client.DisconnectAsync(true);
                client.Dispose();
                return new ResponseData<bool>(false, ex.Message);
            }
            var tasks = new List<Task>();

            foreach (var notification in notificaions)
            {
                try
                {
                    var letter = CreateEmailMessage(notification);
                    await client.SendAsync(letter);

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
            emailMessage.From.Add(new MailboxAddress("Library Admin", _settings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress(notification.Username, notification.Email));
            emailMessage.Subject = "Reminder: Books Not Returned on Time";

            var booksList = string.Join("<li>", notification.ExpiredBooks.Select(book =>
                $"<strong>{book.BookName}</strong> by {book.AuthorName}</li>"));
              
            emailMessage.Body = new TextPart("html")
            {
                Text = $@"
            <html>
            <body>
                <h1>Dear {notification.Username},</h1>
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
