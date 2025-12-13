using LetPortal.Installation.Models;
using LetPortal.Installation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Installation.AppParts.Controllers;

[Route("api/installation")]
[ApiController]
public class InstallationController(IInstallationService installationService) : ControllerBase
{
    /// <summary>
    /// Check the current installation status of the system
    /// </summary>
    /// <returns>Installation status including installed apps and versions</returns>
    [HttpGet("check")]
    [ProducesResponseType(typeof(CheckedInstallationResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckInstallation()
    {
        var result = await installationService.CheckInstallation();
        return Ok(result);
    }

    /// <summary>
    /// Get available versions for a specific app
    /// </summary>
    /// <param name="app">App name (portal or identity)</param>
    /// <returns>List of available versions with their components</returns>
    [HttpGet("versions/{app}")]
    [ProducesResponseType(typeof(AvailableVersionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableVersions(string app)
    {
        if (string.IsNullOrWhiteSpace(app))
        {
            return BadRequest("App name is required.");
        }

        if (app != "portal" && app != "identity")
        {
            return BadRequest("App must be 'portal' or 'identity'.");
        }

        var result = await installationService.GetAvailableVersions(app);
        return Ok(result);
    }

    /// <summary>
    /// Install an app with the specified version
    /// </summary>
    /// <param name="request">Installation request containing app name, version, and database connection details</param>
    /// <returns>Installation result with executed steps</returns>
    [HttpPost("install")]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Install([FromBody] InstallRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.App))
        {
            return BadRequest(InstallationResult.Failed("App name is required."));
        }

        if (request.App != "portal" && request.App != "identity")
        {
            return BadRequest(InstallationResult.Failed("App must be 'portal' or 'identity'."));
        }

        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            return BadRequest(InstallationResult.Failed("Connection string is required."));
        }

        var result = await installationService.Install(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Upgrade an app to the specified version
    /// </summary>
    /// <param name="request">Upgrade request containing app name, target version, and database connection details</param>
    /// <returns>Upgrade result with executed steps</returns>
    [HttpPost("upgrade")]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Upgrade([FromBody] UpgradeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.App))
        {
            return BadRequest(InstallationResult.Failed("App name is required."));
        }

        if (request.App != "portal" && request.App != "identity")
        {
            return BadRequest(InstallationResult.Failed("App must be 'portal' or 'identity'."));
        }

        if (string.IsNullOrWhiteSpace(request.VersionNumber))
        {
            return BadRequest(InstallationResult.Failed("Target version number is required."));
        }

        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            return BadRequest(InstallationResult.Failed("Connection string is required."));
        }

        var result = await installationService.Upgrade(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Uninstall an app
    /// </summary>
    /// <param name="request">Uninstall request containing app name and database connection details</param>
    /// <returns>Uninstall result with executed steps</returns>
    [HttpPost("uninstall")]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Uninstall([FromBody] UninstallRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.App))
        {
            return BadRequest(InstallationResult.Failed("App name is required."));
        }

        if (request.App != "portal" && request.App != "identity")
        {
            return BadRequest(InstallationResult.Failed("App must be 'portal' or 'identity'."));
        }

        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            return BadRequest(InstallationResult.Failed("Connection string is required."));
        }

        var result = await installationService.Uninstall(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Get available patches for a specific app
    /// </summary>
    /// <param name="app">App name (portal or identity)</param>
    /// <returns>List of available patches with their installation status</returns>
    [HttpGet("patches/{app}")]
    [ProducesResponseType(typeof(AvailablePatchesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailablePatches(string app)
    {
        if (string.IsNullOrWhiteSpace(app))
        {
            return BadRequest("App name is required.");
        }

        if (app != "portal" && app != "identity")
        {
            return BadRequest("App must be 'portal' or 'identity'.");
        }

        var result = await installationService.GetAvailablePatches(app);
        return Ok(result);
    }

    /// <summary>
    /// Get available versions for a specific patch
    /// </summary>
    /// <param name="app">App name (portal or identity)</param>
    /// <param name="patchName">Patch name</param>
    /// <returns>List of available versions for the patch</returns>
    [HttpGet("patches/{app}/{patchName}/versions")]
    [ProducesResponseType(typeof(PatchVersionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPatchVersions(string app, string patchName)
    {
        if (string.IsNullOrWhiteSpace(app))
        {
            return BadRequest("App name is required.");
        }

        if (app != "portal" && app != "identity")
        {
            return BadRequest("App must be 'portal' or 'identity'.");
        }

        if (string.IsNullOrWhiteSpace(patchName))
        {
            return BadRequest("Patch name is required.");
        }

        var result = await installationService.GetPatchVersions(app, patchName);
        return Ok(result);
    }

    /// <summary>
    /// Install a patch with the specified version
    /// </summary>
    /// <param name="request">Patch installation request containing app name, patch name, version, and database connection details</param>
    /// <returns>Installation result with executed steps</returns>
    [HttpPost("patch/install")]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(InstallationResult), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InstallPatch([FromBody] InstallPatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.App))
        {
            return BadRequest(InstallationResult.Failed("App name is required."));
        }

        if (request.App != "portal" && request.App != "identity")
        {
            return BadRequest(InstallationResult.Failed("App must be 'portal' or 'identity'."));
        }

        if (string.IsNullOrWhiteSpace(request.PatchName))
        {
            return BadRequest(InstallationResult.Failed("Patch name is required."));
        }

        if (string.IsNullOrWhiteSpace(request.ConnectionString))
        {
            return BadRequest(InstallationResult.Failed("Connection string is required."));
        }

        var result = await installationService.InstallPatch(request);
        
        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }
}
