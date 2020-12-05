using System.Threading.Tasks;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Https;
using LetPortal.Core.Security;
using LetPortal.Identity.Models;
using LetPortal.Identity.Providers.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace LetPortal.Identity.AppParts.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    [OpenApiIgnore]
    public class AccountsController : ControllerBase
    {
        private readonly IIdentityServiceProvider _identityServiceProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountsController(
            IIdentityServiceProvider identityServiceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _identityServiceProvider = identityServiceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenModel), 200)]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            loginModel.ClientIp = _httpContextAccessor.GetClientIpAddress();

            var result = await _identityServiceProvider.SignInAsync(loginModel);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutModel logoutModel)
        {
            await _identityServiceProvider.SignOutAsync(logoutModel);
            return Ok();
        }

        [HttpPost("register")]
        [Authorize(Roles = Roles.BackEndRoles)]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (registerModel.Password != registerModel.Repassword)
            {
                return BadRequest();
            }

            await _identityServiceProvider.RegisterAsync(registerModel);

            return NoContent();
        }

        [HttpGet("refresh/{refreshToken}")]
        [Authorize]
        [ProducesResponseType(typeof(TokenModel), 200)]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var result = await _identityServiceProvider.RefreshTokenAsync(refreshToken);

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _identityServiceProvider.ForgotPasswordAsync(model.Email);

            return NoContent();
        }

        [HttpPost("recovery-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> RecoveryPassword([FromBody] RecoveryPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _identityServiceProvider.RecoveryPasswordAsync(model);

            return NoContent();
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _identityServiceProvider.ChangePasswordAsync(_httpContextAccessor.HttpContext.User.Identity.Name, model);

            return NoContent();
        }
    }
}
