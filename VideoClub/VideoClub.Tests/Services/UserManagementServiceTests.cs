using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using VideoClub.Data.Models;
using VideoClub.Services.Implementations;
using VideoClub.Services.Models.Configuration;
using VideoClub.Services.Models.UserModels;

namespace VideoClub.Tests.Services
{
    public class UserManagementServiceTests
    {
        // Helper method to create a mock UserManager
        private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            return new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);
        }

        // Helper method to create a mock SignInManager
        private Mock<SignInManager<ApplicationUser>> CreateMockSignInManager(Mock<UserManager<ApplicationUser>> mockUserManager)
        {
            var mockContextAccessor = new Mock<IHttpContextAccessor>();
            var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            return new Mock<SignInManager<ApplicationUser>>(
                mockUserManager.Object,
                mockContextAccessor.Object,
                mockClaimsFactory.Object,
                null, null, null, null);
        }

        // Helper method to create a mock IOptions<JwtSettings>
        private IOptions<JwtSettings> CreateMockJwtSettings()
        {
            var jwtSettings = new JwtSettings
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SecretKey = "ThisIsASecretKeyForTesting"
            };

            return Options.Create(jwtSettings);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsSuccessfulResult_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.RegisterUserAsync(new RegisterModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsFailedResult_WhenUserAlreadyExists()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.RegisterUserAsync(new RegisterModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsFailedResult_WhenErrorCreatingUser()
        {
            // Arrange
            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.RegisterUserAsync(new RegisterModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsSuccessfulResultWithToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com" , UserName = "test@example.com" };

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);
            mockSignInManager.Setup(x => x.PasswordSignInAsync(user.Email, It.IsAny<string>(), false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.LoginUserAsync(new LoginModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsFailedResult_WhenUserDoesNotExist()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password" };

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);
            mockSignInManager.Setup(x => x.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false)).ReturnsAsync(SignInResult.Failed);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.LoginUserAsync(new LoginModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsFailedResult_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com" };

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);
            mockSignInManager.Setup(x => x.PasswordSignInAsync(user.Email, It.IsAny<string>(), false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.LoginUserAsync(new LoginModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GrantAdminRoleAsync_ReturnsSuccessfulResult_WhenRoleIsGrantedSuccessfully()
        {
            // Arrange
            var userId = "1";

            var user = new ApplicationUser { Id = userId };

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.GrantAdminRoleAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GrantAdminRoleAsync_ReturnsFailedResult_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.GrantAdminRoleAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description == "User not found.");
        }

        [Fact]
        public async Task GrantAdminRoleAsync_ReturnsFailedResult_WhenUserIsAlreadyAdmin()
        {
            // Arrange
            var userId = "1";

            var user = new ApplicationUser { Id = userId };

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User is already in this role." }));

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.GrantAdminRoleAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description == "User is already in this role.");
        }

        [Fact]
        public async Task RemoveAdminRoleAsync_ReturnsSuccessfulResult_WhenRoleIsRevokedSuccessfully()
        {
            // Arrange
            var userId = "1";

            var user = new ApplicationUser { Id = userId };

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.RemoveAdminRoleAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RemoveAdminRoleAsync_ReturnsFailedResult_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.RemoveAdminRoleAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description == "User not found.");
        }

        [Fact]
        public async Task RemoveAdminRoleAsync_ReturnsFailedResult_WhenUserIsNotAdmin()
        {
            // Arrange
            var userId = "1";

            var user = new ApplicationUser { Id = userId };

            var mockUserManager = CreateMockUserManager();
            mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            mockUserManager.Setup(x => x.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User is not in this role." }));

            var mockSignInManager = CreateMockSignInManager(mockUserManager);

            var mockJwtSettings = CreateMockJwtSettings();

            var service = new UserManagementService(mockUserManager.Object, mockSignInManager.Object, mockJwtSettings);

            // Act
            var result = await service.RemoveAdminRoleAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description == "User is not in this role.");
        }
    }
}