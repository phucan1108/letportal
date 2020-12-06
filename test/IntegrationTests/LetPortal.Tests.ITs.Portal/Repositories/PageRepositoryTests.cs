using LetPortal.Core.Security;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Repositories.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Portal.Repositories
{
    public class PageRepositoryTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public PageRepositoryTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Get_One_By_Name_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            PageMongoRepository pageRepository = new PageMongoRepository(_context.GetMongoConnection());
#pragma warning restore CA2000 // Dispose objects before losing scope
            // Act
            Page pageBuilderTest = new Page
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "page-builder",
                DisplayName = "Page Builder",
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            await pageRepository.AddAsync(pageBuilderTest);

            Page result = await pageRepository.GetOneByNameAsync("page-builder");
            pageRepository.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Get_All_Short_Pages_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            PageMongoRepository pageRepository = new PageMongoRepository(_context.GetMongoConnection());
#pragma warning restore CA2000 // Dispose objects before losing scope
            // Act
            Page pageBuilderTest = new Page
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "page-builder-1",
                DisplayName = "Page Builder",
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            await pageRepository.AddAsync(pageBuilderTest);

            List<LetPortal.Portal.Models.Pages.ShortPageModel> result = await pageRepository.GetAllShortPagesAsync();
            pageRepository.Dispose();
            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Get_Short_Portal_Claims_Model_In_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            PageMongoRepository pageRepository = new PageMongoRepository(_context.GetMongoConnection());
#pragma warning restore CA2000 // Dispose objects before losing scope
            // Act
            Page pageBuilderTest = new Page
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "page-builder-2",
                DisplayName = "Page Builder",
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                }
            };

            await pageRepository.AddAsync(pageBuilderTest);

            List<LetPortal.Portal.Models.Pages.ShortPortalClaimModel> result = await pageRepository.GetShortPortalClaimModelsAsync();
            pageRepository.Dispose();
            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Get_One_By_Name_For_Render_Mongo_Test()
        {
            if(!_context.AllowMongoDB)
            {
                Assert.True(true);
                return;
            }
            // Arrange
#pragma warning disable CA2000 // Dispose objects before losing scope
            PageMongoRepository pageRepository = new PageMongoRepository(_context.GetMongoConnection());
#pragma warning restore CA2000 // Dispose objects before losing scope

            // Act
            Page pageBuilderTest = new Page
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = "page-builder-3",
                DisplayName = "Page Builder",
                UrlPath = "portal/page/builder",
                ShellOptions = new List<ShellOption>(),
                Claims = new List<PortalClaim>
                {
                    PortalClaimStandards.AllowAccess
                },
                PageDatasources = new List<PageDatasource>
                {
                    new PageDatasource
                    {
                        Id = "asd",
                        IsActive = true,
                        Name = "data",
                        Options = new LetPortal.Portal.Entities.Shared.DatasourceOptions
                        {
                            Type = LetPortal.Portal.Entities.Shared.DatasourceControlType.Database,
                            DatabaseOptions = new LetPortal.Portal.Entities.Shared.SharedDatabaseOptions
                            {
                                DatabaseConnectionId = "sfasfasf",
                                Query = "{\"$query\":{\"databases\":[{\"$match\":{\"_id\":\"ObjectId('{{queryparams.id}}')\"}}]}}"
                            }
                        }
                    }
                },
                Commands = new List<PageButton>
                {
                    new PageButton
                    {
                        Id = "180b55dd-e31f-387f-0844-7a9c74f88941",
                        Name = "Save",
                        Icon = "save",
                        Color = "primary",
                        AllowHidden = "!!queryparams.id",
                        IsRequiredValidation = true,
                        ButtonOptions = new ButtonOptions
                        {
                            ActionCommandOptions = new ActionCommandOptions
                            {
                                ActionType = ActionType.ExecuteDatabase,
                                DbExecutionChains = new DatabaseExecutionChains
                                {
                                    Steps = new List<DatabaseExecutionStep>
                                    {
                                        new DatabaseExecutionStep
                                        {
                                            DatabaseConnectionId = "fasdfas",
                                            ExecuteCommand = "{\"$insert\":{\"{{options.entityname}}\":{ \"$data\": \"{{data}}\"}}}"
                                        }
                                    }
                                },
                                NotificationOptions = new NotificationOptions
                                {
                                    CompleteMessage = "Insert new database successfully!",
                                    FailedMessage = "Oops! Something went wrong, please try again!"
                                },
                                ConfirmationOptions = new ConfirmationOptions
                                {
                                    IsEnable = true,
                                    ConfirmationText = "Are you sure to create a database?"
                                }
                            },
                            RouteOptions = new RouteOptions
                            {
                                IsEnable = false
                            }
                        }
                    }
                }
            };

            await pageRepository.AddAsync(pageBuilderTest);
            Page page = await pageRepository.GetOneByNameForRenderAsync("page-builder-3");
            pageRepository.Dispose();
            // Assert
            Assert.NotNull(page);
        }
    }
}
