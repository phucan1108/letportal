using LetPortal.Identity.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace LetPortal.Tests.ITs.Identity
{
    public class FakeSignInManager : SignInManager<User>
    {
        public FakeSignInManager
            (
                UserManager<User> userManager,
                IHttpContextAccessor contextAccessor,
                IUserClaimsPrincipalFactory<User> claimsFactory,
                IOptions<IdentityOptions> optionsAccessor,
                ILogger<SignInManager<User>> logger,
                IAuthenticationSchemeProvider schemes,
                IUserConfirmation<User> userConfirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, userConfirmation)
        {
        }

        public override Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            if(user.Username == "admin")
            {
                return password == "@Dm1n!" ? Task.FromResult(SignInResult.Success) : Task.FromResult(SignInResult.Failed);
            }

            return Task.FromResult(SignInResult.Failed);
        }
    }
}
