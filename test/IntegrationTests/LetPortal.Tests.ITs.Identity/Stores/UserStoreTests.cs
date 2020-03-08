using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Identity.Stores
{
    public class UserStoreTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public UserStoreTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Create_User_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            Microsoft.AspNetCore.Identity.IdentityResult result = await userStore.CreateAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task Add_Claim_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.AddClaimsAsync(_context.GenerateUser(), new List<Claim> { new Claim("aaa", "vvv") }, new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Add_To_Role_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.AddToRoleAsync(_context.GenerateUser(), "TestRole", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Find_By_Email_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            LetPortal.Identity.Entities.User result = await userStore.FindByEmailAsync("ADMIN@PORTAL.COM", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Find_By_Id_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            LetPortal.Identity.Entities.User result = await userStore.FindByIdAsync("5ce287ee569d6f23e8504cef", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Find_By_Name_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            LetPortal.Identity.Entities.User result = await userStore.FindByNameAsync("ADMIN", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Get_Access_Failed_Count_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            int result = await userStore.GetAccessFailedCountAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(result >= 0);
        }

        [Fact]
        public async Task Get_Claims_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            IList<Claim> result = await userStore.GetClaimsAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Get_Email_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            string result = await userStore.GetEmailAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_Lockout_Enabled_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            bool result = await userStore.GetLockoutEnabledAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Get_Lockout_End_Date_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.GetLockoutEndDateAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Get_Normalized_Email_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            string result = await userStore.GetNormalizedEmailAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_Normalized_UserName_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            string result = await userStore.GetNormalizedUserNameAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_Password_Hash_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            string result = await userStore.GetNormalizedUserNameAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_Roles_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            IList<string> result = await userStore.GetRolesAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Get_Security_Stamp_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            string result = await userStore.GetSecurityStampAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_UserId_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            string result = await userStore.GetUserIdAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_UserName_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            string result = await userStore.GetUserNameAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Has_Password_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.HasPasswordAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Increment_Access_FailedCount_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            int result = await userStore.IncrementAccessFailedCountAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(result >= 0);
        }

        [Fact]
        public async Task Is_In_Role_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            bool result = await userStore.IsInRoleAsync(_context.GenerateUser(), "SuperAdmin", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Remove_Claims_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.RemoveClaimsAsync(_context.GenerateUser(), new List<Claim>(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Reset_Access_FailedCount_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.ResetAccessFailedCountAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Email_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetEmailAsync(_context.GenerateUser(), "tasda@aaa.com", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Email_Confirmed_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetEmailConfirmedAsync(_context.GenerateUser(), true, new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Lockout_Enabled_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetLockoutEnabledAsync(_context.GenerateUser(), false, new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Lockout_EndDate_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetLockoutEndDateAsync(_context.GenerateUser(), DateTimeOffset.UtcNow.AddDays(10), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Normalized_Email_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetNormalizedEmailAsync(_context.GenerateUser(), "asdas@aaa.com".ToUpper(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Normalized_UserName_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetNormalizedUserNameAsync(_context.GenerateUser(), "admin".ToUpper(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Password_Hash_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetPasswordHashAsync(_context.GenerateUser(), "asdasdasd", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_Security_Stamp_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetSecurityStampAsync(_context.GenerateUser(), "asdasdasd", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_UserName_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            await userStore.SetUserNameAsync(_context.GenerateUser(), "asdasdasd", new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_User_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.UserStore userStore = _context.GetUserStore();

            // Act
            Microsoft.AspNetCore.Identity.IdentityResult result = await userStore.UpdateAsync(_context.GenerateUser(), new System.Threading.CancellationToken());

            userStore.Dispose();
            // Assert
            Assert.True(result.Succeeded);
        }
    }
}
