using System;
using System.Threading.Tasks;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Utils;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Models;
using LetPortal.Identity.Repositories.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace LetPortal.Identity.AppParts.Controllers
{
    [Route("api/usersessions")]
    [ApiController]
    [OpenApiIgnore]
    public class UserSessionController : ControllerBase
    {
        private readonly IUserSessionRepository _userSessionRepository;

        public UserSessionController(IUserSessionRepository userSessionRepository)
        {
            _userSessionRepository = userSessionRepository;
        }

        [Authorize]
        [HttpPost("add")]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> AddActivity([FromBody] UserActivityModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _userSessionRepository.LogUserActivityAsync(model.UserSessionId, new UserActivity
            {
                Id = DataUtil.GenerateUniqueId(),
                ActivityName = model.ActivityName,
                ActivityType = model.ActivityType,
                ActivityDate = DateTime.UtcNow,
                Content = model.Content
            });

            return NoContent();
        }
    }
}
