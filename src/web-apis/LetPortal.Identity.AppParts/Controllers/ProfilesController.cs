using System.Threading.Tasks;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Models;
using LetPortal.Identity.Providers.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace LetPortal.Identity.AppParts.Controllers
{
    [Route("api/profiles")]
    [ApiController]
    [OpenApiIgnore]
    public class ProfilesController : ControllerBase
    {
        private readonly IIdentityServiceProvider _identityServiceProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfilesController(
            IIdentityServiceProvider identityServiceProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _identityServiceProvider = identityServiceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(ProfileModel), 200)]
        public async Task<IActionResult> GetProfile()
        {
            return Ok(await _identityServiceProvider.GetUserProfile(_httpContextAccessor.HttpContext.User.Identity.Name));
        }

        [HttpPost("update")]
        [Authorize]
        public async Task<IActionResult> UpdateProfiles([FromBody] ProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _identityServiceProvider.AddClaimsAsync(
                _httpContextAccessor.HttpContext.User.Identity.Name,
                new System.Collections.Generic.List<Identity.Entities.BaseClaim>
                {
                   StandardClaims.FullName(model.FullName),
                   StandardClaims.Avatar(model.Avatar)
                });

            return NoContent();
        }
    }
}
