using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Hovedopgave.Server.Controllers
{
    [ApiController]
    [EnableCors("FrontEndUI")]
    [Route("[controller]")]
    public class AdminRightsController : ControllerBase
    {
        private readonly AdminRightsServices adminRightsServices;

        public AdminRightsController()
        {
            adminRightsServices = new AdminRightsServices();
        }

        [HttpGet("hello")]
        public IActionResult HelloWorld()
        {
            return Ok("Hello, World!");
        }

        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {

            try
            {
                List<UserDTO> admins = await adminRightsServices.GetAllAdmins();

                return Ok(admins);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users.", details = ex.Message });
            }
        }

        [HttpGet("search-users/{displayName}")]
        public async Task<IActionResult> SearchActiveUsers(string displayName, [FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                List<UserDTO> users = await adminRightsServices.SearchActiveUsers(displayName, page, pageSize);

                if (users == null || users.Count == 0)
                {
                    return NotFound(new { message = $"No active users found with display name '{displayName}'." });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving active users.", details = ex.Message });
            }
        }

        [HttpGet("search-deleted-users")]
        public async Task<IActionResult> GetDeletedUsers([FromQuery] string? displayName, [FromQuery] int page, [FromQuery] int pageSize)
        {
            try
            {
                List<UserDTO> users = await adminRightsServices.SearchDeletedUsers(displayName, page, pageSize);

                if (users == null || users.Count == 0)
                {
                    return NotFound(new { message = $"No deleted users found with display name '{displayName}'." });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving deleted users.", details = ex.Message });
            }
        }

        [HttpPut("soft-delete/{displayName}")]
        public async Task<IActionResult> SoftDeleteUser(string displayName, [FromBody] LoggedInUser loggedInUser) 
        {
            try
            {
                bool success = await adminRightsServices.SoftDeleteUser(loggedInUser.LoggedInUserID, displayName);

                if (success)
                {
                    return Ok(new { message = $"User with display name '{displayName}' was successfully deleted." });
                }
                else
                {
                    return NotFound(new { message = $"User with display name '{displayName}' not found or already deleted." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the user.", details = ex.Message });
            }
        }


        [HttpPut("update-role/{role}/name/{displayName}")]
        public async Task<IActionResult> UpdateUsersRole(string displayName, Roles.Role role, [FromBody] LoggedInUser loggedInUser)
        {
            try
            {
                bool success = await adminRightsServices.UpdateUsersRole(loggedInUser.LoggedInUserID, displayName, role);
                Console.WriteLine("Logged in User: " + loggedInUser.LoggedInUserID);
                
                if (success)
                {
                    return Ok(new { message = $"'{displayName}'s role was successfully changed to '{role}'." });
                }
                else
                {
                    return NotFound(new { message = $"Not enough privileges to change '{displayName}'s role" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating users role.", details = ex.Message });
            }
        }


        [HttpPut("update-user/{displayName}")]
        public async Task<IActionResult> UpdateUserDetails(string displayName, [FromBody] UserDTO user)
        {
            try
            {
                bool success = await adminRightsServices.UpdateUserDetails(user.LoggedInUser, user);

                if (success)
                {
                    return Ok(new { message = $"User '{displayName}' details were successfully updated." });
                }
                else
                {
                    return NotFound(new { message = $"Not enough privileges to update user '{displayName}' details." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating user details.", details = ex.Message });
            }
        }

        [HttpPut("reset-password/{displayName}")]
        public async Task<IActionResult> ResetUserPassword(string displayName)
        {
            try
            {
                bool success = await adminRightsServices.ResetUserPassword(displayName);

                if (success)
                {
                    return Ok(new { message = $"Password for user '{displayName}' has been reset and sent via email." });
                }
                else
                {
                    return NotFound(new { message = $"User with display name '{displayName}' not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while resetting the password.", details = ex.Message });
            }
        }

        [HttpDelete("hard-delete/{displayName}")]
        public async Task<IActionResult> HardDeleteUser(string displayName, [FromBody] LoggedInUser loggedInUser)
        {
            try
            {
                bool success = await adminRightsServices.HardDeleteUser(loggedInUser.LoggedInUserID, displayName);

                if (success)
                {
                    return Ok(new { message = $"User with display name '{displayName}' was hard deleted." });
                }
                else
                {
                    return NotFound(new { message = "Not enough privileges to hard delete user." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while hard deleting the user.", details = ex.Message });
            }
        }

        [HttpPut("undelete-user/{displayName}")]
        public async Task<IActionResult> UndeleteUser(string displayName, [FromBody] LoggedInUser loggedInUser)
        {
            try
            {
                bool success = await adminRightsServices.UndeleteUser(loggedInUser.LoggedInUserID, displayName);

                if (success)
                {
                    return Ok(new { message = $"User with display name '{displayName}' was successfully restored." });
                }
                else
                {
                    return NotFound(new { message = $"Not enough privileges to undelete user '{displayName}' or user not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while undeleting the user.", details = ex.Message });
            }
        }


    }
}
