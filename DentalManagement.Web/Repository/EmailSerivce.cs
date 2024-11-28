using MimeKit;
using System.Configuration;
using MailKit.Net.Smtp;
using MimeKit;

namespace DentalManagement.Web.Repository
{
    public class EmailSerivce
    {
        private readonly IConfiguration _configuration;

        public EmailSerivce(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail , string subject , string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(
                _configuration["EmailSettings:SenderName"],
                _configuration["EmailSettings:SenderEmail"]
                ));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration["EmailSettings:SMTPServer"],
                                int.Parse(_configuration["EmailSettings:Port"]),
                                MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(
              _configuration["EmailSettings:SenderEmail"],
              _configuration["EmailSettings:Password"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }

}
