using LetPortal.Identity.Configurations;
using LetPortal.Identity.Models;
using System.Threading.Tasks;

namespace LetPortal.Identity.Providers.Emails
{
    public interface IEmailServiceProvider
    {
        Task SendEmailAsync(EmailEnvelop emailEnvelop, EmailOptions emailOptions = null);
    }
}
