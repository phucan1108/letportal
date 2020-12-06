using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Models.Shared;
using LetPortal.Portal.Providers.Components;
using LetPortal.Portal.Providers.Databases;
using LetPortal.Portal.Providers.Pages;
using LetPortal.Portal.Repositories.Components;
using LetPortal.Portal.Repositories.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.Portal.AppParts.Controllers
{
    [Route("api/pages")]
    [ApiController]
    public class PagesController : ControllerBase
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IStandardServiceProvider _standardServiceProvider;

        private readonly IPageServiceProvider _pageServiceProvider;

        private readonly IPageRepository _pageRepository;

        private readonly ICompositeControlRepository _controlRepository;

        private readonly IServiceLogger<PagesController> _logger;

        public PagesController(
            IPageRepository pageRepository,
            IDatabaseServiceProvider databaseServiceProvider,
            IStandardServiceProvider standardServiceProvider,
            IPageServiceProvider pageServiceProvider,
            ICompositeControlRepository controlRepository,
            IServiceLogger<PagesController> logger)
        {
            _pageRepository = pageRepository;
            _controlRepository = controlRepository;
            _databaseServiceProvider = databaseServiceProvider;
            _standardServiceProvider = standardServiceProvider;
            _pageServiceProvider = pageServiceProvider;
            _logger = logger;
        }

        [HttpGet("shorts")]
        [ProducesResponseType(typeof(List<ShortPageModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetAllShortPages()
        {
            var result = await _pageRepository.GetAllShortPagesAsync();
            _logger.Info("All short pages: {@result}", result);
            return Ok(result);
        }

        [HttpGet("all-claims")]
        [ProducesResponseType(typeof(List<ShortPortalClaimModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetAllPortalClaims()
        {
            var result = await _pageRepository.GetShortPortalClaimModelsAsync();
            _logger.Info("All portal claims: {@result}", result);
            return Ok(result);
        }

        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(typeof(Page), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetOneById(string id)
        {
            var result = await _pageRepository.GetOneAsync(id);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpGet("{name}")]
        [ProducesResponseType(typeof(Page), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetOne(string name)
        {
            var result = await _pageRepository.GetOneByNameAsync(name);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpGet("render/{name}")]
        [ProducesResponseType(typeof(Page), 200)]
        [Authorize]
        public async Task<IActionResult> GetOneForRender(string name)
        {
            var result = await _pageRepository.GetOneByNameForRenderAsync(name);
            _logger.Info("Found page: {@result}", result);
            return Ok(result);
        }

        [HttpGet("short-pages")]
        [ProducesResponseType(typeof(IEnumerable<ShortEntityModel>), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> GetShortPages([FromQuery] string keyWord = null)
        {
            return Ok(await _pageRepository.GetShortPages(keyWord));
        }

        [HttpPost("")]
        [ProducesResponseType(typeof(string), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Create([FromBody] Page page)
        {
            if (ModelState.IsValid)
            {
                page.Id = DataUtil.GenerateUniqueId();
                await _pageRepository.AddAsync(page);
                _logger.Info("Created page: {@page}", page);
                return Ok(page.Id);
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Update(string id, [FromBody] Page page)
        {
            if (ModelState.IsValid)
            {
                page.Id = id;
                await _pageRepository.UpdateAsync(id, page);
                _logger.Info("Updated page: {@page}", page);
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Delete(string id)
        {
            await _pageRepository.DeleteAsync(id);
            _logger.Info("Deleted page id: {id}", id);
            return Ok();
        }

        [HttpGet("check-exist/{name}")]
        [ProducesResponseType(typeof(bool), 200)]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> CheckExist(string name)
        {
            return Ok(await _pageRepository.IsExistAsync(a => a.Name == name));
        }

        [HttpPost("{pageId}/submit")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        [Authorize]
        public async Task<IActionResult> SubmitCommand(string pageId, [FromBody] PageSubmittedButtonModel pageSubmittedButtonModel)
        {
            var page = await _pageRepository.GetOneAsync(pageId);
            if (page != null)
            {
                var button = page.Commands.First(a => a.Name == pageSubmittedButtonModel.ButtonName);
                if (button.ButtonOptions.ActionCommandOptions.ActionType == Portal.Entities.Shared.ActionType.ExecuteDatabase)
                {
                    var result =
                        await _databaseServiceProvider
                                .ExecuteDatabase(
                                    button.ButtonOptions.ActionCommandOptions.DbExecutionChains,
                                    pageSubmittedButtonModel
                                        .Parameters
                                        .Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue }),
                                    pageSubmittedButtonModel.LoopDatas?.Select(a => new LoopDataParamModel
                                    {
                                        Name = a.Name,
                                        Parameters = a.Parameters.Select(b => b.Select(c => new ExecuteParamModel { Name = c.Name, RemoveQuotes = c.RemoveQuotes, ReplaceValue = c.ReplaceValue }).ToList()).ToList()
                                    }));

                    return Ok(result);
                }
            }

            return NotFound();
        }

        [HttpPost("{pageId}/fetch-control-datasource")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        [Authorize]
        public async Task<IActionResult> FetchControlDatasource(string pageId, [FromBody] PageSelectionDatasourceModel model)
        {
            var page = await _pageRepository.GetOneAsync(pageId);
            if (page != null)
            {
                var section = page.Builder.Sections.First(a => a.Name == model.SectionName);
                if (section.ConstructionType == SectionContructionType.Standard)
                {
                    // Only support Standard component
                    var sectionStandard = (await _standardServiceProvider.GetStandardComponentsByIds(new List<string> { section.ComponentId })).First();

                    if (!model.IsChildCompositeControl)
                    {
                        var control = sectionStandard.Controls.First(a => a.Name == model.ControlName);
                        if (control.Type == Portal.Entities.SectionParts.Controls.ControlType.AutoComplete
                            || control.Type == Portal.Entities.SectionParts.Controls.ControlType.Select)
                        {
                            var parameters = model?
                                               .Parameters
                                               .Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue }).ToList();

                            var result =
                               await _databaseServiceProvider
                                       .ExecuteDatabase(
                                           control.DatasourceOptions.DatabaseOptions.DatabaseConnectionId,
                                           control.DatasourceOptions.DatabaseOptions.Query,
                                           parameters
                                           );

                            return Ok(result);
                        }
                        return BadRequest();
                    }
                    else
                    {
                        var compositeControl = await _controlRepository.GetOneAsync(model.CompositeControlId);
                        var control = compositeControl.Controls.First(a => a.Name == model.ControlName);
                        if (control.Type == Portal.Entities.SectionParts.Controls.ControlType.AutoComplete
                           || control.Type == Portal.Entities.SectionParts.Controls.ControlType.Select)
                        {
                            var parameters = model?
                                               .Parameters
                                               .Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue }).ToList();

                            var result =
                               await _databaseServiceProvider
                                       .ExecuteDatabase(
                                           control.DatasourceOptions.DatabaseOptions.DatabaseConnectionId,
                                           control.DatasourceOptions.DatabaseOptions.Query,
                                           parameters
                                           );

                            return Ok(result);
                        }
                        return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }

            return NotFound();
        }

        [HttpPost("{pageId}/trigger-control-event")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        [Authorize]
        public async Task<IActionResult> ExecuteTriggeredEvent(string pageId, [FromBody] PageTriggeringEventModel model)
        {
            var page = await _pageRepository.GetOneAsync(pageId);
            if (page != null)
            {
                var section = page.Builder.Sections.First(a => a.Name == model.SectionName);
                if (section.ConstructionType == SectionContructionType.Standard)
                {
                    // Only support Standard component
                    var sectionStandard = (await _standardServiceProvider.GetStandardComponentsByIds(new List<string> { section.ComponentId })).First();

                    var control = sectionStandard.Controls.First(a => a.Name == model.ControlName);
                    var triggeringEvent = control.PageControlEvents.FirstOrDefault(a => a.EventName == model.EventName);

                    if (triggeringEvent != null && triggeringEvent.EventActionType == Portal.Entities.Components.Controls.EventActionType.QueryDatabase)
                    {
                        var parameters = model?
                                           .Parameters
                                           .Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue }).ToList();

                        var result =
                           await _databaseServiceProvider
                                   .ExecuteDatabase(
                                       triggeringEvent.EventDatabaseOptions.DatabaseConnectionId,
                                       triggeringEvent.EventDatabaseOptions.Query,
                                       parameters
                                       );

                        return Ok(result);
                    }
                    return BadRequest();
                }
                else
                {
                    return BadRequest();
                }
            }

            return NotFound();
        }

        [HttpPost("{pageId}/async-validator")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        [Authorize]
        public async Task<IActionResult> ExecuteAsyncValidator(string pageId, [FromBody] PageAsyncValidatorModel validatorModel)
        {
            var page = await _pageRepository.GetOneAsync(pageId);
            if (page != null)
            {
                var section = page.Builder.Sections.First(a => a.Name == validatorModel.SectionName);
                if (section.ConstructionType == SectionContructionType.Standard)
                {
                    // Only support Standard component
                    var sectionStandard = (await _standardServiceProvider.GetStandardComponentsByIds(new List<string> { section.ComponentId })).First();

                    var control = sectionStandard.Controls.First(a => a.Name == validatorModel.ControlName);
                    var asyncValidator = control.AsyncValidators.First(a => a.ValidatorName == validatorModel.AsyncName);
                    if (asyncValidator.AsyncValidatorOptions.ValidatorType == Portal.Entities.Components.Controls.AsyncValidatorType.DatabaseValidator)
                    {
                        var result =
                            await _databaseServiceProvider
                                    .ExecuteDatabase(
                                        asyncValidator.AsyncValidatorOptions.DatabaseOptions.DatabaseConnectionId,
                                        asyncValidator.AsyncValidatorOptions.DatabaseOptions.Query,
                                        validatorModel?
                                            .Parameters
                                            .Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue }));

                        return Ok(result);
                    }

                    return BadRequest();
                }
                else
                {
                    return BadRequest();
                }
            }

            return NotFound();
        }

        [HttpPost("{pageId}/fetch-datasource")]
        [ProducesResponseType(typeof(ExecuteDynamicResultModel), 200)]
        [Authorize]
        public async Task<IActionResult> GetDatasourceForPage(string pageId, [FromBody] PageRequestDatasourceModel pageRequestDatasourceModel)
        {
            var page = await _pageRepository.GetOneAsync(pageId);
            if (page != null)
            {
                var datasource = page.PageDatasources.First(a => a.Id == pageRequestDatasourceModel.DatasourceId);
                if (datasource.Options.Type == Portal.Entities.Shared.DatasourceControlType.Database)
                {
                    var result = await _databaseServiceProvider.ExecuteDatabase(datasource.Options.DatabaseOptions.DatabaseConnectionId, datasource.Options.DatabaseOptions.Query, pageRequestDatasourceModel.Parameters.Select(a => new ExecuteParamModel { Name = a.Name, RemoveQuotes = a.RemoveQuotes, ReplaceValue = a.ReplaceValue }));

                    return Ok(result);
                }
            }

            return NotFound();
        }


        [HttpPost("clone")]
        [Authorize(Roles = Roles.BackEndRoles)]
        public async Task<IActionResult> Clone([FromBody] CloneModel model)
        {
            _logger.Info("Requesting clone page with {@model}", model);
            await _pageRepository.CloneAsync(model.CloneId, model.CloneName);
            return Ok();
        }

        [HttpGet("{id}/languages")]
        [Authorize(Roles = Roles.BackEndRoles)]
        [ProducesResponseType(typeof(List<LanguageKey>), 200)]
        public async Task<IActionResult> GenerateLanguages(string id)
        {
            _logger.Info("Requesting generate languages {@id}", id);
            return Ok(await _pageServiceProvider.GetPageLanguages(id));
        }
    }
}
