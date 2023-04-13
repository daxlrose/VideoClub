using Microsoft.AspNetCore.Identity;
using VideoClub.Services.Models.UserModels;

namespace VideoClub.Services.Contracts
{
    public interface IUserManagementService
    {
        Task<IdentityResult> RegisterUserAsync(RegisterModel model);
        Task<string> LoginUserAsync(LoginModel model);
        Task<IdentityResult> GrantAdminRoleAsync(string userId);
        Task<IdentityResult> RemoveAdminRoleAsync(string userId);
    }
}
