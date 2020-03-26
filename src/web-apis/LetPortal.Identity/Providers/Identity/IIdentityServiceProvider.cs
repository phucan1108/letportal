using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Models;

namespace LetPortal.Identity.Providers.Identity
{
    public interface IIdentityServiceProvider
    {
        Task RegisterAsync(RegisterModel registerModel);

        Task<TokenModel> SignInAsync(LoginModel loginModel);

        Task SignOutAsync(LogoutModel logoutModel);

        Task<TokenModel> RefreshTokenAsync(string refreshToken);

        Task ForgotPasswordAsync(string email);

        Task RecoveryPasswordAsync(RecoveryPasswordModel recoveryPasswordModel);

        Task ChangePasswordAsync(string userName,ChangePasswordModel resetPasswordModel);

        Task<List<Role>> GetRolesAsync();

        Task AddPortalClaimsToRoleAsync(string roleName, List<PortalClaimModel> portalClaims);

        Task<List<RolePortalClaimModel>> GetPortalClaimsByRoleAsync(string roleName);

        Task<List<RolePortalClaimModel>> GetPortalClaimsAsync(string username);

        Task<ProfileModel> GetUserProfile(string username);

        Task AddClaimsAsync(string userName, List<BaseClaim> claims);
    }
}
