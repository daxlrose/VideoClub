using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoClub.Services.Models.UserModels;
using VideoClub.Services.Contracts;
using VideoClub.Services.Implementations;

namespace VideoClub.Api.Controllers
{
    /// <summary>
    /// Provides actions for user management, authentication, and role-based operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UsersController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The user registration model.</param>
        /// <returns>An IActionResult representing the result of the registration process.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Invalid input.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManagementService.RegisterUserAsync(model);

                if (result.Succeeded)
                {
                    return Ok("User registered successfully.");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="model">The user login model.</param>
        /// <returns>An IActionResult containing the JWT token or an error message.</returns>
        /// <response code="200">User logged in successfully.</response>
        /// <response code="400">Invalid input or invalid login attempt.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var jwt = await _userManagementService.LoginUserAsync(model);

                if (!string.IsNullOrEmpty(jwt))
                {
                    return Ok(new { Token = jwt });
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Grants the "Admin" role to a user.
        /// </summary>
        /// <param name="userId">The ID of the user to grant the "Admin" role.</param>
        /// <returns>An IActionResult representing the result of the role assignment process.</returns>
        /// <response code="200">Admin role granted successfully.</response>
        /// <response code="400">Invalid input or user not found.</response>
        [HttpPost("grantadmin/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GrantAdminRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var result = await _userManagementService.GrantAdminRoleAsync(userId);

            if (result.Succeeded)
            {
                return Ok("Admin role granted successfully.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Revokes the "Admin" role from a user.
        /// </summary>
        /// <param name="userId">The ID of the user to revoke the "Admin" role.</param>
        /// <returns>An IActionResult representing the result of the role revocation process.</returns>
        /// <response code="200">Admin role revoked successfully.</response>
        /// <response code="400">Invalid input or user not found.</response>
        [HttpPost("revokeadmin/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevokeAdminRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var result = await _userManagementService.RemoveAdminRoleAsync(userId);

            if (result.Succeeded)
            {
                return Ok("Admin role revoked successfully.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Deletes a user with the given ID.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <returns>An IActionResult representing the result of the deletion process.</returns>
        /// <response code="200">User deleted successfully.</response>
        /// <response code="400">Invalid input or user not found.</response>
        /// <response code="403">Unauthorized access. Only Admin users can perform this action.</response>
        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var result = await _userManagementService.DeleteUserAsync(userId);

            if (result.Succeeded)
            {
                return Ok("User deleted successfully.");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }
    }
}
