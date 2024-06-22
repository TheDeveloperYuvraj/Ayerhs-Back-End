using Ayerhs.Core.Entities.Utility;
using Ayerhs.Core.Interfaces.Utility;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Ayerhs.Application.Services.Utility
{
    /// <summary>
    /// Service class responsible for sending emails, specifically OTP (One-Time Password) emails.
    /// Implements the <see cref="IEmailService"/> interface.
    /// </summary>
    public class EmailService(IOptions<SmtpSettings> smtpSettings, ILogger<EmailService> logger) : IEmailService
    {
        private readonly SmtpSettings _smtpSettings = smtpSettings.Value;
        private readonly ILogger<EmailService> _logger = logger;

        /// <summary>
        /// Asynchronously sends an email containing a One-Time Password (OTP) to the specified recipient.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="otp">The One-Time Password to include in the email body.</param>
        /// <param name="body">The custom body content of the email. If not provided, a default message with the OTP code will be used.</param>
        /// <returns>An asynchronous task representing the email sending operation.</returns>
        public async Task SendOtpEmailAsync(string email, string otp, string body)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
            }

            if (string.IsNullOrEmpty(otp))
            {
                throw new ArgumentException($"'{nameof(otp)}' cannot be null or empty.", nameof(otp));
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("YourAppName", _smtpSettings.Username));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Your OTP Code";

            message.Body = new TextPart("plain")
            {
                Text = string.IsNullOrEmpty(body) ? $"Your OTP code is {otp}" : body.Replace("{otp}", otp)
            };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, false);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("OTP email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email} because {Message}", email, ex.Message);
            }
        }
    }
}
