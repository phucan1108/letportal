using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Identity.Configurations;
using LetPortal.Identity.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LetPortal.Identity.Providers.Emails
{
    public class EmailServiceProvider : IEmailServiceProvider
    {
        private readonly IOptionsMonitor<EmailOptions> _emailOptions;
        private readonly ILogger _logger;

        public EmailServiceProvider(ILoggerFactory loggerFactory, IOptionsMonitor<EmailOptions> emailOptions)
        {
            _logger = loggerFactory.CreateLogger<EmailServiceProvider>();
            _emailOptions = emailOptions;
        }

        public Task SendEmailAsync(EmailEnvelop emailEnvelop, EmailOptions emailOptions = null)
        {
            if (emailOptions == null)
                emailOptions = _emailOptions.CurrentValue;

            if (emailOptions.SkipMode)
            {
                return Task.CompletedTask;
            }

            var client = new SmtpClient(emailOptions.Host, emailOptions.Port)
            {
                Credentials = new NetworkCredential(emailOptions.UserName, emailOptions.Password),
                EnableSsl = emailOptions.EnableSSL
            };
            return client.SendMailAsync(
                new MailMessage(emailOptions.From, emailEnvelop.To, emailEnvelop.Subject, emailEnvelop.Body)
                { IsBodyHtml = true }
            );
        }
    }
}
