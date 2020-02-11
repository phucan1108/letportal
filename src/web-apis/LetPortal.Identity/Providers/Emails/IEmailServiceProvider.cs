using System.Threading.Tasks;
using LetPortal.Identity.Configurations;
using LetPortal.Identity.Models;

namespace LetPortal.Identity.Providers.Emails
{
    public interface IEmailServiceProvider
    {
        Task SendEmailAsync(EmailEnvelop emailEnvelop, EmailOptions emailOptions = null);
    }
}
