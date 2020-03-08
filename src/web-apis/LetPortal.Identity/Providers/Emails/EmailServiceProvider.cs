using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using LetPortal.Identity.Configurations;
using LetPortal.Identity.Models;
using Microsoft.Extensions.Options;

namespace LetPortal.Identity.Providers.Emails
{
    public class EmailServiceProvider : IEmailServiceProvider
    {
        private readonly IOptionsMonitor<EmailOptions> _emailOptions;

        public EmailServiceProvider(IOptionsMonitor<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions;
        }

        public async Task SendEmailAsync(EmailEnvelop emailEnvelop, EmailOptions emailOptions = null)
        {
            if (emailOptions == null)
            {
                emailOptions = _emailOptions.CurrentValue;
            }

            if (emailOptions.SkipMode)
            {
                return;
            }

            var client = new SmtpClient(emailOptions.Host, emailOptions.Port)
            {
                Credentials = new NetworkCredential(emailOptions.UserName, emailOptions.Password),
                EnableSsl = emailOptions.EnableSSL
            };

#pragma warning disable CA2000 // Dispose objects before losing scope
            await client.SendMailAsync(
                new MailMessage(emailOptions.From, emailEnvelop?.To, emailEnvelop.Subject, emailEnvelop.Body)
                { IsBodyHtml = true }
            ).ConfigureAwait(false);
#pragma warning restore CA2000 // Dispose objects before losing scope

            client.Dispose();
        }
    }
}
