﻿using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Exceptions;
using LetPortal.Identity;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Models;
using LetPortal.Identity.Providers.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.IdentityApis.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IIdentityServiceProvider _identityServiceProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public RolesController(IIdentityServiceProvider identityServiceProvider, IHttpContextAccessor httpContextAccessor)
        {
            _identityServiceProvider = identityServiceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("")]        
        [ProducesResponseType(typeof(List<Role>), 200)]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        [Authorize(Roles = Constants.BACK_END_ROLES)]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(await _identityServiceProvider.GetRolesAsync());
        }

        [HttpPut("{roleName}/claims/portal")]
        [Authorize(Roles = Constants.BACK_END_ROLES)]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> AddPortalClaims(string roleName, [FromBody] List<PortalClaimModel> portalClaimModels)
        {
            await _identityServiceProvider.AddPortalClaimsToRoleAsync(roleName, portalClaimModels);
            return Ok();
        }

        [HttpGet("portal-claims")]
        [Authorize]
        [ProducesResponseType(typeof(List<RolePortalClaimModel>), 200)]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> GetPortalClaims()
        {
            return Ok(await _identityServiceProvider.GetPortalClaimsAsync(_httpContextAccessor.HttpContext.User.Identity.Name));
        }

        [HttpGet("{roleName}/claims")]
        [Authorize]
        [ProducesResponseType(typeof(List<RolePortalClaimModel>), 200)]
        [ProducesResponseType(typeof(ErrorCode), 500)]
        public async Task<IActionResult> GetPortalClaimsByRole(string roleName)
        {
            return Ok(await _identityServiceProvider.GetPortalClaimsByRoleAsync(roleName));
        }
    }
}
