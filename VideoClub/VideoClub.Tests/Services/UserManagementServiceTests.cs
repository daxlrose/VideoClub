using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using VideoClub.Data.Models;
using VideoClub.Services.Contracts;
using VideoClub.Services.Implementations;
using VideoClub.Services.Models.Configuration;
using VideoClub.Services.Models.UserModels;

namespace VideoClub.Tests.Services
{
    public class UserManagementServiceTests : IDisposable
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly IOptions<JwtSettings> _mockJwtSettings;
        private readonly IUserManagementService _service;

        public UserManagementServiceTests()
        {
            _mockUserManager = CreateMockUserManager();
            _mockSignInManager = CreateMockSignInManager(_mockUserManager);
            _mockJwtSettings = CreateMockJwtSettings();
            _service = new UserManagementService(_mockUserManager.Object, _mockSignInManager.Object, _mockJwtSettings);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsSuccessfulResult_WhenRegistrationIsSuccessful()
        {
            // Arrange
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.RegisterUserAsync(new RegisterModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsFailedResult_WhenUserAlreadyExists()
        {
            // Arrange
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            // Act
            var result = await _service.RegisterUserAsync(new RegisterModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterUserAsync_ReturnsFailedResult_WhenErrorCreatingUser()
        {
            // Arrange
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            // Act
            var result = await _service.RegisterUserAsync(new RegisterModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsSuccessfulResultWithToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com" , UserName = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockSignInManager.Setup(x => x.PasswordSignInAsync(user.Email, It.IsAny<string>(), false, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(new List<string>());

            // Act
            var result = await _service.LoginUserAsync(new LoginModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsFailedResult_WhenUserDoesNotExist()
        {
            // Arrange
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(loginModel.Email, loginModel.Password, false, false)).ReturnsAsync(SignInResult.Failed);

            var mockJwtSettings = CreateMockJwtSettings();

            // Act
            var result = await _service.LoginUserAsync(new LoginModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginUserAsync_ReturnsFailedResult_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new ApplicationUser { Email = "test@example.com" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(user.Email, It.IsAny<string>(), false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _service.LoginUserAsync(new LoginModel { Email = "test@example.com", Password = "P@ssw0rd" });

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GrantAdminRoleAsync_ReturnsSuccessfulResult_WhenRoleIsGrantedSuccessfully()
        {
            // Arrange
            var userId = "1";

            var user = new ApplicationUser { Id = userId };

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.GrantAdminRoleAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task GrantAdminRoleAsync_ReturnsFailedResult_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _service.GrantAdminRoleAsync(userId);

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

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User is already in this role." }));

            // Act
            var result = await _service.GrantAdminRoleAsync(userId);

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

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _service.RemoveAdminRoleAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RemoveAdminRoleAsync_ReturnsFailedResult_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "1";

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _service.RemoveAdminRoleAsync(userId);

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

            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mockUserManager.Setup(x => x.RemoveFromRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User is not in this role." }));

            // Act
            var result = await _service.RemoveAdminRoleAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Description == "User is not in this role.");
        }

        public void Dispose()
        {
            _mockUserManager.Reset();
            _mockSignInManager.Reset();
        }

        private Mock<UserManager<ApplicationUser>> CreateMockUserManager()
        {
            return new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null);
        }

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
    }
}