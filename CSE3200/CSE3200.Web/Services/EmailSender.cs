// Services/EmailSender.cs
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace CSE3200.Web.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = htmlMessage
                };

                using (var client = new SmtpClient())
                {
                    // For Gmail, we need to use StartTLS
                    await client.ConnectAsync(_emailSettings.Server, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                    // Note: since we don't have an OAuth2 token, disable the XOAUTH2 authentication mechanism
                    client.AuthenticationMechanisms.Remove("XOAUTH2");

                    if (!string.IsNullOrEmpty(_emailSettings.Username))
                    {
                        await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                    }

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {email}");
                throw new ApplicationException($"Email sending failed: {ex.Message}");
            }
        }
    }

    public class EmailSettings
    {
        public string Server { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public string SenderName { get; set; } = "Disaster Management System";
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; } = true;
    }
}