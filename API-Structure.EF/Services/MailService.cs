using API_Structure.Core.Helpers;
using API_Structure.Core.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Runtime;

namespace API_Structure.EF.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailSettings.FromName,_mailSettings.FromAddress));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Confirm Your Email";

            var confirmationUrl = $"{_mailSettings.ConfirmationUrl}?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            message.Body = new TextPart("html")
            {
                Text = $@"<h1>Email Confirmation</h1>
                     <p>Please confirm your email by <a href='{confirmationUrl}'>clicking here</a></p>
                     <p>Or copy this link to your browser: {confirmationUrl}</p>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _mailSettings.SmtpServer, _mailSettings.SmtpPort, SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_mailSettings.SmtpUsername, _mailSettings.SmtpPassword);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

        }
    }
}
