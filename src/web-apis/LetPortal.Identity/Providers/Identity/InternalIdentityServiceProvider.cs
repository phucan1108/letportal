using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Configurations;
using LetPortal.Core.Extensions;
using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Exceptions;
using LetPortal.Identity.Models;
using LetPortal.Identity.Providers.Emails;
using LetPortal.Identity.Repositories.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LetPortal.Identity.Providers.Identity
{
    public class InternalIdentityServiceProvider : IIdentityServiceProvider
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly RoleManager<Role> _roleManager;

        private readonly IOptionsMonitor<JwtBearerOptions> _jwtBearerOptions;

        private readonly IIssuedTokenRepository _issuedTokenRepository;

        private readonly IUserSessionRepository _userSessionRepository;

        private readonly IRoleRepository _roleRepository;

        private readonly IEmailServiceProvider _emailServiceProvider;

        private readonly IServiceLogger<InternalIdentityServiceProvider> _serviceLogger;

        public InternalIdentityServiceProvider(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IOptionsMonitor<JwtBearerOptions> jwtBearerOptions,
            IIssuedTokenRepository issuedTokenRepository,
            IUserSessionRepository userSessionRepository,
            IRoleRepository roleRepository,
            IEmailServiceProvider emailServiceProvider,
            IServiceLogger<InternalIdentityServiceProvider> serviceLogger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtBearerOptions = jwtBearerOptions;
            _issuedTokenRepository = issuedTokenRepository;
            _userSessionRepository = userSessionRepository;
            _roleRepository = roleRepository;
            _emailServiceProvider = emailServiceProvider;
            _roleManager = roleManager;
            _serviceLogger = serviceLogger;
        }

        public async Task RegisterAsync(RegisterModel registerModel)
        {
            var result = await _userManager.CreateAsync(new User
            {
                Username = registerModel.Username,
                Email = registerModel.Email,
                Claims = new List<BaseClaim>
                {
                    StandardClaims.Sub(registerModel.Username),
                    StandardClaims.FullName(registerModel.Username),
                    StandardClaims.AccessAppSelectorPage
                }
            }, registerModel.Password);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(a => a.Code == "DuplicateUserName"))
                {
                    throw new IdentityException(ErrorCodes.UsernameHasBeenRegistered);
                }
                else
                {
                    throw new IdentityException(new Core.Exceptions.ErrorCode { MessageCode = result.Errors.First().Code, MessageContent = result.Errors.First().Description });
                }
            }
        }

        public async Task<TokenModel> SignInAsync(LoginModel loginModel)
        {
            _serviceLogger.Info("User Login {$loginModel}", loginModel.ToJson());
            var user = await _userManager.FindByNameAsync(loginModel.Username);

            if (user != null)
            {
                var validationResult = await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
                if (validationResult.Succeeded)
                {
                    var userClaims = await _userManager.GetClaimsAsync(user);

                    var token = SignedToken(userClaims);

                    // Create UserSession
                    var userSession = new UserSession
                    {
                        Id = DataUtil.GenerateUniqueId(),
                        RequestIpAddress = loginModel.ClientIp,
                        SoftwareAgent = loginModel.SoftwareAgent,
                        InstalledVersion = loginModel.VersionInstalled,
                        SignInDate = DateTime.UtcNow,
                        UserId = user.Id,
                        Username = user.Username,
                        UserActivities = new List<UserActivity>
                    {
                        new UserActivity
                        {
                            Id = DataUtil.GenerateUniqueId(),
                            ActivityName = "USER_SIGNIN",
                            Content = "Signin successfully",
                            ActivityDate = DateTime.UtcNow,
                            ActivityType = ActivityType.Info
                        }
                    }
                    };
                    await _userSessionRepository.AddAsync(userSession);

                    // Create refresh token
                    var expToken = DateTime.UtcNow.AddMinutes(_jwtBearerOptions.CurrentValue.TokenExpiration);
                    var expRefreshToken = DateTime.UtcNow.AddMinutes(_jwtBearerOptions.CurrentValue.RefreshTokenExpiration);
                    var issuedToken = new IssuedToken
                    {
                        Id = DataUtil.GenerateUniqueId(),
                        UserId = user.Id,
                        JwtToken = token,
                        ExpiredJwtToken = expToken,
                        ExpiredRefreshToken = expRefreshToken,
                        RefreshToken = CryptoUtil.ToSHA256(Guid.NewGuid().ToString()),
                        UserSessionId = userSession.Id,
                        Deactive = false
                    };

                    await _issuedTokenRepository.AddAsync(issuedToken);
                    _serviceLogger.Info("User Login Successfully {$issuedToken}", issuedToken.ToJson());
                    return new TokenModel
                    {
                        Token = token,
                        Exp = ((DateTimeOffset)expToken).ToUnixTimeSeconds(),
                        ExpRefresh = ((DateTimeOffset)expRefreshToken).ToUnixTimeSeconds(),
                        RefreshToken = issuedToken.RefreshToken,
                        UserSessionId = userSession.Id
                    };
                }
            }

            throw new IdentityException(ErrorCodes.CannotSignIn);
        }   

        public async Task SignOutAsync(LogoutModel logoutModel)
        {
            // In fact, we just add a signout date for auditing
            // Because JWT is helping us to validate service-self instead of connecting back to Identity Server
            // For micro-services, we need to implement distributed JWT cache for signing out all sessions

            var userSession = await _userSessionRepository.GetOneAsync(logoutModel.UserSession);

            userSession.SignOutDate = DateTime.UtcNow;
            userSession.AlreadySignOut = true;
            await _userSessionRepository.UpdateAsync(userSession.Id, userSession);
        }

        public async Task<TokenModel> RefreshTokenAsync(string refreshToken)
        {
            var issuedToken = await _issuedTokenRepository.GetByRefreshToken(refreshToken);
            var canDeactive = await _issuedTokenRepository.DeactiveRefreshToken(refreshToken);
            if (canDeactive)
            {
                var claimPrincipal = GetPrincipalFromExpiredToken(issuedToken.JwtToken);
                var newToken = SignedToken(claimPrincipal.Claims);

                // Create refresh token
                var expToken = DateTime.UtcNow.AddMinutes(_jwtBearerOptions.CurrentValue.TokenExpiration);
                var expRefreshToken = DateTime.UtcNow.AddMinutes(_jwtBearerOptions.CurrentValue.RefreshTokenExpiration);
                var newIssuedToken = new IssuedToken
                {
                    Id = DataUtil.GenerateUniqueId(),
                    UserId = issuedToken.Id,
                    JwtToken = newToken,
                    ExpiredJwtToken = expToken,
                    ExpiredRefreshToken = expRefreshToken,
                    RefreshToken = CryptoUtil.ToSHA256(Guid.NewGuid().ToString()),
                    UserSessionId = issuedToken.UserSessionId,
                    Deactive = false
                };

                await _issuedTokenRepository.AddAsync(newIssuedToken);

                return new TokenModel
                {
                    Token = newToken,
                    Exp = ((DateTimeOffset)expToken).ToUnixTimeSeconds(),
                    ExpRefresh = ((DateTimeOffset)expRefreshToken).ToUnixTimeSeconds(),
                    RefreshToken = newIssuedToken.RefreshToken,
                    UserSessionId = newIssuedToken.UserSessionId
                };
            }

            throw new IdentityException(ErrorCodes.CannotDeactiveRefreshToken);
        }

        private string SignedToken(IEnumerable<Claim> userClaims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerOptions.CurrentValue.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _jwtBearerOptions.CurrentValue.Issuer,
                audience: _jwtBearerOptions.CurrentValue.Audience,
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(_jwtBearerOptions.CurrentValue.TokenExpiration),
                signingCredentials: credentials
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerOptions.CurrentValue.Secret)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var verifyingUser = await _userManager.FindByEmailAsync(email);

            if (verifyingUser != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(verifyingUser);

                await _emailServiceProvider.SendEmailAsync(new EmailEnvelop
                {
                    To = email,
                    Subject = "Let Portal - Forgot Password",
                    Body = string.Format("{0}/#?user={1}&code={2}", "http://localhost:4200/forgot-password", verifyingUser.Id, token)
                });
            }
        }

        public async Task RecoveryPasswordAsync(RecoveryPasswordModel recoveryPasswordModel)
        {
            var verifyingUser = await _userManager.FindByIdAsync(recoveryPasswordModel.UserId);

            if (verifyingUser != null)
            {
                var result = await _userManager.ResetPasswordAsync(verifyingUser, recoveryPasswordModel.ValidateCode,
                    recoveryPasswordModel.NewPassword);

                if (!result.Succeeded)
                {
                    throw new IdentityException(new Core.Exceptions.ErrorCode { MessageCode = result.Errors.First().Code, MessageContent = result.Errors.First().Description });
                }
            }
        }

        public Task<List<Role>> GetRolesAsync()
        {
            return Task.FromResult(_roleRepository.GetAsQueryable().ToList());
        }

        public async Task AddPortalClaimsToRoleAsync(string roleName, List<PortalClaimModel> portalClaims)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            role.Claims = portalClaims.Select(a => a.ToBaseClaim()).ToList();
            await _roleRepository.UpdateAsync(role.Id, role);
        }

        public async Task<List<RolePortalClaimModel>> GetPortalClaimsAsync(string username)
        {
            var roles = (await _userManager.FindByNameAsync(username)).Roles;

            var dicClaims = await _roleRepository.GetBaseClaims(roles.ToArray());

            var claims = dicClaims.SelectMany(a => a.Value).GroupBy(a => a.ClaimType).Select(g => new RolePortalClaimModel { Name = g.Key, Claims = g.Select(k => k.ClaimValue).Distinct().ToList() }).ToList();

            return claims;
        }

        public async Task<List<RolePortalClaimModel>> GetPortalClaimsByRoleAsync(string roleName)
        {
            var dicClaims = await _roleRepository.GetBaseClaims(new string[] { roleName });

            var claims = dicClaims.Where(b => b.Value != null).SelectMany(a => a.Value).GroupBy(a => a.ClaimType).Select(g => new RolePortalClaimModel { Name = g.Key, Claims = g.Select(k => k.ClaimValue).Distinct().ToList() }).ToList();

            return claims;
        }

        public async Task ChangePasswordAsync(string userName, ChangePasswordModel resetPasswordModel)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user != null)
            {
                if(resetPasswordModel.NewPassword.Equals(resetPasswordModel.ReNewPassword, StringComparison.Ordinal))
                {
                    var result = await _userManager.ChangePasswordAsync(user, resetPasswordModel.CurrentPassword, resetPasswordModel.NewPassword);
                    if (!result.Succeeded)
                    {
                        throw new IdentityException(ErrorCodes.CannotChangePassword);
                    }
                }
                else
                {
                    throw new IdentityException(ErrorCodes.PasswordDoesNotMatchWithRePassword);
                }
            }
            else
            {
                throw new IdentityException(ErrorCodes.WrongUserName);
            }
        }

        public async Task AddClaimsAsync(string userName, List<BaseClaim> claims)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if(user != null)
            {
                var result = await _userManager.AddClaimsAsync(user, claims.Select(a => a.ToClaim()));
                if (!result.Succeeded)
                {
                    throw new IdentityException(ErrorCodes.CannotUpdateClaimsOfUser);
                }
            }
            else
            {
                throw new IdentityException(ErrorCodes.WrongUserName);
            }
        }

        public async Task<ProfileModel> GetUserProfile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var fullnameClaim = claims.FirstOrDefault(a => a.Type == StandardClaims.DefaultFullNameClaim);
                var avatarClaim = claims.FirstOrDefault(a => a.Type == StandardClaims.DefaultAvatarClaim);
                return new ProfileModel
                {
                    FullName = fullnameClaim?.Value,
                    Avatar = avatarClaim?.Value
                };
            }
            else
            {
                throw new IdentityException(ErrorCodes.WrongUserName);
            }
        }
    }
}
