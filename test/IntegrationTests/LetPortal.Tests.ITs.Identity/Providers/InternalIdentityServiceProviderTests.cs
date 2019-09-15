using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Providers.Identity;
using LetPortal.Identity.Repositories.Identity;
using LetPortal.Identity.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
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

        private UserManager<User> getUserManager()
        {
            var databaseOptions = _context.MongoDatabaseOptions;
            var databaseOptionsMock = Mock.Of<IOptionsMonitor<DatabaseOptions>>(_ => _.CurrentValue == databaseOptions);
            var userRepository = new UserMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            var roleRepository = new RoleMongoRepository(new MongoConnection(databaseOptionsMock.CurrentValue));
            var userStore = new UserStore(userRepository, roleRepository);
            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            var identityOptions = new IdentityOptions
            {

            };
            identityOptionsMock.Setup(a => a.Value).Returns(identityOptions);
            var userValidators = new List<IUserValidator<User>>();
            var validator = new Mock<IUserValidator<User>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<User>>();
            pwdValidators.Add(new PasswordValidator<User>());
            var userManager = new UserManager<User>(
                userStore,
                identityOptionsMock.Object,
                new PasswordHasher<User>(),
                userValidators,
                 pwdValidators,
                 new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<User>>>().Object);

            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<User>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();


            return userManager;
        }
    }
}
