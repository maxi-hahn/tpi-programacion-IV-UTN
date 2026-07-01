using Application.Interfaces;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Service {  
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();

            email.From.Add(
                MailboxAddress.Parse("Gymtupproject@gmail.com"));

            email.To.Add(
                MailboxAddress.Parse(to));

            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            var logPath = "smtp.log";
            using var protocolLogger = new ProtocolLogger(logPath);
            using var smtp = new SmtpClient(protocolLogger);

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            try
            {
                File.AppendAllText(logPath, $"Auth mechanisms: {string.Join(", ", smtp.AuthenticationMechanisms)}{Environment.NewLine}");
            }
            catch
            {
            }

            try
            {
                await smtp.AuthenticateAsync(
                  _settings.Email,
                  _settings.Password);
            }
            catch (AuthenticationException ex)
            {
                try
                {
                    File.AppendAllText(logPath, $"Authentication failed: {ex.Message}{Environment.NewLine}");
                    if (ex.InnerException != null)
                        File.AppendAllText(logPath, $"Inner: {ex.InnerException.Message}{Environment.NewLine}");
                }
                catch
                {
                    // ignore logging errors
                }

                throw;
            }

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}