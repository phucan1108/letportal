using LetPortal.Core.Configurations;
using LetPortal.Core.Logger;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Configurations;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Models;
using LetPortal.Identity.Providers.Emails;
using LetPortal.Identity.Providers.Identity;
using LetPortal.Identity.Repositories.Identity;
using LetPortal.Identity.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Identity.Providers
{
    public class InternalIdentityServiceProviderTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public InternalIdentityServiceProviderTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Register_User_Mongo_Test()
        {
            // Arrange
            InternalIdentityServiceProvider internalISProvider = GetIdentityServiceProvider();

            // Act
            await internalISProvider.RegisterAsync(new LetPortal.Identity.Models.RegisterModel
            {
                Username = "testuser",
                Password = "@Dm1n!",
                Repassword = "@Dm1n!",
                Email = "testuser@gmail.com"
            });

            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Sign_In_Mongo_Test()
        {
            // Arrange
            InternalIdentityServiceProvider internalISProvider = GetIdentityServiceProvider();

            // Act
            TokenModel result = await internalISProvider.SignInAsync(new LetPortal.Identity.Models.LoginModel
            {
                Username = "admin",
                Password = "@Dm1n!"
            });

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Refresh_Token_Mongo_Test()
        {
            // Arrange
            InternalIdentityServiceProvider internalISProvider = GetIdentityServiceProvider();

            // Act
            TokenModel tokenModel = await internalISProvider.SignInAsync(new LetPortal.Identity.Models.LoginModel
            {
                Username = "admin",
                Password = "@Dm1n!"
            });

            TokenModel result = await internalISProvider.RefreshTokenAsync(tokenModel.RefreshToken);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Get_Roles_Mongo_Test()
        {
            // Arrange
            InternalIdentityServiceProvider internalISProvider = GetIdentityServiceProvider();

            // Act
            List<Role> result = await internalISProvider.GetRolesAsync();

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Add_Portal_Claims_To_Role_Mongo_Test()
        {
            // Arrange
            InternalIdentityServiceProvider internalISProvider = GetIdentityServiceProvider();

            // Act
            await internalISProvider.AddPortalClaimsToRoleAsync("SuperAdmin", new List<PortalClaimModel>
            {
                new PortalClaimModel
                {
                    Name = "testclaim",
                    Value = "test"
                }
            });

            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Get_Portal_Claims_By_Role_Mongo_Test()
        {
            // Arrange
            InternalIdentityServiceProvider internalISProvider = GetIdentityServiceProvider();

            // Act
            List<RolePortalClaimModel> result = await internalISProvider.GetPortalClaimsByRoleAsync("SuperAdmin");

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Get_Portal_Claims_Mongo_Test()
        {
            // Arrange
            InternalIdentityServiceProvider internalISProvider = GetIdentityServiceProvider();

            // Act
            List<RolePortalClaimModel> result = await internalISProvider.GetPortalClaimsAsync("admin");

            // Assert
            Assert.NotEmpty(result);
        }

        private InternalIdentityServiceProvider GetIdentityServiceProvider()
        {
            JwtBearerOptions jwtOptions = new JwtBearerOptions
            {
                Audience = "LetPortal",
                Issuer = "letportal.app",
                RefreshTokenExpiration = 45,
                TokenExpiration = 30,
                Secret = "9f3acfa82146f5e4a7dabf17c2b63f538c0bcffb8872e889367df2e2c23cef94"
            };

            IOptionsMonitor<JwtBearerOptions> mockJwtOptions = Mock.Of<IOptionsMonitor<JwtBearerOptions>>(_ => _.CurrentValue == jwtOptions);

            DatabaseOptions databaseOptions = _context.MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
#pragma warning disable CA2000 // Dispose objects before losing scope
            IssuedTokenMongoRepository issuedTokenRepository = new IssuedTokenMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            UserSessionMongoRepository userSessionRepository = new UserSessionMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            RoleMongoRepository roleRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
#pragma warning restore CA2000 // Dispose objects before losing scope   

            Mock<IEmailServiceProvider> mockEmailServiceProvider = new Mock<IEmailServiceProvider>();
            mockEmailServiceProvider
                .Setup(a => a.SendEmailAsync(It.IsAny<EmailEnvelop>(), It.IsAny<EmailOptions>()))
                .Returns(Task.CompletedTask);
            Mock<IServiceLogger<InternalIdentityServiceProvider>> mockServiceLogger = new Mock<IServiceLogger<InternalIdentityServiceProvider>>();

#pragma warning disable CA2000 // Dispose objects before losing scope
            UserManager<User> userManager = GetUserManager();
#pragma warning restore CA2000 // Dispose objects before losing scope
            InternalIdentityServiceProvider internalISProvider = new InternalIdentityServiceProvider(
                userManager,
                GetSignInManager(userManager),
#pragma warning disable CA2000 // Dispose objects before losing scope
                roleManager: GetRoleManager(),
#pragma warning restore CA2000 // Dispose objects before losing scope
                mockJwtOptions,
                issuedTokenRepository,
                userSessionRepository,
                roleRepository,
                mockEmailServiceProvider.Object,
                mockServiceLogger.Object);

            return internalISProvider;
        }

        private RoleManager<Role> GetRoleManager()
        {
            DatabaseOptions databaseOptions = _context.MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
#pragma warning disable CA2000 // Dispose objects before losing scope
            RoleMongoRepository roleRepositoryMongo = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            RoleStore roleStore = new RoleStore(roleRepositoryMongo);
#pragma warning restore CA2000 // Dispose objects before losing scope
            List<IRoleValidator<Role>> roleValidators = new List<IRoleValidator<Role>>();
            Mock<IRoleValidator<Role>> roleValidatorMock = new Mock<IRoleValidator<Role>>();
            roleValidatorMock.Setup(a => a.ValidateAsync(It.IsAny<RoleManager<Role>>(), It.IsAny<Role>())).Returns(Task.FromResult(IdentityResult.Success));
            roleValidators.Add(roleValidatorMock.Object);
            Mock<ILookupNormalizer> lookupNormalizedMock = new Mock<ILookupNormalizer>();
            lookupNormalizedMock.Setup(a => a.NormalizeName(It.IsAny<string>())).Returns((string name) => name.ToUpper());
            lookupNormalizedMock.Setup(a => a.NormalizeEmail(It.IsAny<string>())).Returns((string name) => name.ToUpper());

            RoleManager<Role> roleManager = new RoleManager<Role>(
                roleStore,
                roleValidators,
                lookupNormalizedMock.Object,
                new IdentityErrorDescriber(),
                new Mock<ILogger<RoleManager<Role>>>().Object);
            return roleManager;
        }

        private UserManager<User> GetUserManager()
        {
            DatabaseOptions databaseOptions = _context.MongoDatabaseOptions;
            IOptionsMonitor<DatabaseOptions> databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
#pragma warning disable CA2000 // Dispose objects before losing scope
            UserMongoRepository userRepository = new UserMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            RoleMongoRepository roleRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            UserStore userStore = new UserStore(userRepository, roleRepository);
#pragma warning restore CA2000 // Dispose objects before losing scope
            Mock<IOptions<IdentityOptions>> identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            IdentityOptions identityOptions = new IdentityOptions
            {
                SignIn = new SignInOptions
                {
                    RequireConfirmedEmail = false,
                    RequireConfirmedPhoneNumber = false
                },
                User = new UserOptions
                {
                    RequireUniqueEmail = true
                },
                Lockout = new LockoutOptions
                {
                    AllowedForNewUsers = true
                }
            };
            identityOptionsMock.Setup(a => a.Value).Returns(identityOptions);
            List<IUserValidator<User>> userValidators = new List<IUserValidator<User>>();
            List<PasswordValidator<User>> pwdValidators = new List<PasswordValidator<User>>();

            Mock<IServiceProvider> serviceProviders = new Mock<IServiceProvider>();
            Mock<IUserTwoFactorTokenProvider<User>> mockUserFactorTokenProvider = new Mock<IUserTwoFactorTokenProvider<User>>();
            mockUserFactorTokenProvider
                .Setup(a => a.CanGenerateTwoFactorTokenAsync(It.IsAny<UserManager<User>>(), It.IsAny<User>()))
                .Returns(Task.FromResult(true));

            mockUserFactorTokenProvider
                .Setup(a => a.GenerateAsync(It.IsAny<string>(), It.IsAny<UserManager<User>>(), It.IsAny<User>()))
                .Returns(Task.FromResult(Guid.NewGuid().ToString()));

            mockUserFactorTokenProvider
                .Setup(a => a.ValidateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserManager<User>>(), It.IsAny<User>()))
                .Returns(Task.FromResult(true));

            UserManager<User> userManager = new UserManager<User>(
                userStore,
                identityOptionsMock.Object,
                new PasswordHasher<User>(),
                userValidators,
                pwdValidators,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                serviceProviders.Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            return userManager;
        }

        private SignInManager<User> GetSignInManager(UserManager<User> userManager)
        {
            IdentityOptions identityOptions = new IdentityOptions
            {
                SignIn = new SignInOptions
                {
                    RequireConfirmedEmail = false,
                    RequireConfirmedPhoneNumber = false
                },
                User = new UserOptions
                {
                    RequireUniqueEmail = true
                },
                Lockout = new LockoutOptions
                {
                    AllowedForNewUsers = true
                }
            };

            Mock<IOptions<IdentityOptions>> mockIdentityOptions = new Mock<IOptions<IdentityOptions>>();
            mockIdentityOptions.Setup(a => a.Value).Returns(identityOptions);
            Mock<IHttpContextAccessor> mockHttpContext = new Mock<IHttpContextAccessor>();
            mockHttpContext
                .Setup(a => a.HttpContext)
                .Returns(new DefaultHttpContext());

            Mock<IUserClaimsPrincipalFactory<User>> mockUserClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            mockUserClaimsPrincipalFactory
                .Setup(a => a.CreateAsync(It.IsAny<User>()))
                .Returns(Task.FromResult(new ClaimsPrincipal()));

            Mock<ILogger<SignInManager<User>>> mockLogger = new Mock<ILogger<SignInManager<User>>>();
            AuthenticationOptions authenticationOptions = new AuthenticationOptions
            {

            };
            Mock<IOptions<AuthenticationOptions>> mockAuthOptions = new Mock<IOptions<AuthenticationOptions>>();
            mockAuthOptions.Setup(a => a.Value).Returns(authenticationOptions);
            AuthenticationSchemeProvider authenticationSchemeProvider = new AuthenticationSchemeProvider(mockAuthOptions.Object);
            Mock<IUserConfirmation<User>> mockUserConfirmation = new Mock<IUserConfirmation<User>>();
            mockUserConfirmation.Setup(a => a.IsConfirmedAsync(It.IsAny<UserManager<User>>(), It.IsAny<User>())).Returns(Task.FromResult(true));
            FakeSignInManager signInManager = new FakeSignInManager(
                userManager,
                mockHttpContext.Object,
                mockUserClaimsPrincipalFactory.Object,
                mockIdentityOptions.Object,
                mockLogger.Object,
                authenticationSchemeProvider,
                mockUserConfirmation.Object
                );

            return signInManager;
        }
    }
}
