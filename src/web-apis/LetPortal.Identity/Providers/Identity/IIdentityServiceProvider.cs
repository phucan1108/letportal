using LetPortal.Identity.Entities;
using LetPortal.Identity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Identity.Providers.Identity
{
    public interface IIdentityServiceProvider
    {
        Task RegisterAsync(RegisterModel registerModel);

        Task<TokenModel> SignInAsync(LoginModel loginModel);

        Task<TokenModel> RefreshTokenAsync(string refreshToken);

        Task ForgotPassword(string email);

        Task RecoveryPassword(RecoveryPasswordModel recoveryPasswordModel);

        Task<List<Role>> GetRolesAsync();

        Task AddPortalClaimsToRoleAsync(string roleName, List<PortalClaimModel> portalClaims);

        Task<List<RolePortalClaimModel>> GetPortalClaimsByRole(string roleName);

        Task<List<RolePortalClaimModel>> GetPortalClaims(string username);
    }
}
