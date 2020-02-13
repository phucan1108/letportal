using System.Threading.Tasks;
using Xunit;

namespace LetPortal.Tests.ITs.Identity.Stores
{
    public class RoleStoreTests : IClassFixture<IntegrationTestsContext>
    {
        private readonly IntegrationTestsContext _context;

        public RoleStoreTests(IntegrationTestsContext context)
        {
            _context = context;
        }

        [Fact]
        public async Task Create_Role_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            await roleStore.CreateAsync(_context.GenerateRole(), new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Add_Claims_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            await roleStore.AddClaimAsync(_context.GenerateRole(), new System.Security.Claims.Claim("Aaa", "asdas"), new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Delete_Role_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();
            LetPortal.Identity.Entities.Role role = _context.GenerateRole();
            // Act
            await roleStore.CreateAsync(role, new System.Threading.CancellationToken());

            await roleStore.DeleteAsync(role, new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Find_By_Id_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            LetPortal.Identity.Entities.Role result = await roleStore.FindByIdAsync("5c06a15e4cc9a850bca44488", new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Find_By_Name_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            LetPortal.Identity.Entities.Role result = await roleStore.FindByNameAsync("SUPERADMIN", new System.Threading.CancellationToken());

            roleStore.Dispose();

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Get_Claims_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            System.Collections.Generic.IList<System.Security.Claims.Claim> result = await roleStore.GetClaimsAsync(_context.GenerateRole(), new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task Get_Normalized_Role_Name_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            string result = await roleStore.GetNormalizedRoleNameAsync(_context.GenerateRole(), new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_Role_Id_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            string result = await roleStore.GetRoleIdAsync(_context.GenerateRole(), new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Get_Role_Name_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            string result = await roleStore.GetRoleNameAsync(_context.GenerateRole(), new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(!string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task Remove_Claim_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            await roleStore.RemoveClaimAsync(_context.GenerateRole(), new System.Security.Claims.Claim("apps", "5c162e9005924c1c741bfd22"), new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Set_RoleName_Mongo_Test()
        {
            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();

            // Act
            await roleStore.SetRoleNameAsync(_context.GenerateRole(), "aaa", new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(true);
        }

        [Fact]
        public async Task Update_Role_Mongo_Test()
        {

            // Arrange
            LetPortal.Identity.Stores.RoleStore roleStore = _context.GetRoleStore();
            LetPortal.Identity.Entities.Role role = _context.GenerateRole();
            // Act
            await roleStore.CreateAsync(role, new System.Threading.CancellationToken());

            role.DisplayName = "ab";
            await roleStore.UpdateAsync(role, new System.Threading.CancellationToken());

            roleStore.Dispose();
            // Assert
            Assert.True(true);
        }
    }
}
