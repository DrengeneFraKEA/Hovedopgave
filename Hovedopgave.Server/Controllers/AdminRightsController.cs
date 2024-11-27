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

        [HttpGet("display-name/{displayName}")]
        public async Task<IActionResult> GetUserByDisplayName(string displayName)
        {
            try
            {
                List<UserDTO> users = await adminRightsServices.GetUserByDisplayName(displayName);

                if (users == null)
                {
                    return NotFound(new { message = $"User with display name '{displayName}' not found." });
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the user.", details = ex.Message });
            }
        }

        [HttpPut("soft-delete/{displayName}")]
        public async Task<IActionResult> SoftDeleteUser(string displayName) 
        {
            try
            {
                bool success = await adminRightsServices.SoftDeleteUser(displayName);

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


        [HttpPut("update-role/{role}/user/{displayName}")]
        public async Task<IActionResult> UpdateUsersRole(string displayName, Roles.RoleDB role)
        {
            try
            {
                bool success = await adminRightsServices.UpdateUsersRole(displayName, role);

                if (success)
                {
                    return Ok(new { message = $"'{displayName}'s role was successfully changed to '{role}'." });
                }
                else
                {
                    return NotFound(new { message = $"User with display name '{displayName}' not found or already deleted." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating users role.", details = ex.Message });
            }
        }


        [HttpPut("update-name/{newDisplayName}/user/{displayName}")]
        public async Task<IActionResult> UpdateUsersDisplayName(string displayName, string newDisplayName)
        {
            try
            {
                bool success = await adminRightsServices.UpdateUsersDisplayName(displayName, newDisplayName);

                if (success)
                {
                    return Ok(new { message = $"'{displayName}'s display name was successfully changed to '{newDisplayName}'." });
                }
                else
                {
                    return NotFound(new { message = $"Display name '{newDisplayName}' already exists" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating users display name.", details = ex.Message });
            }
        }


    }
}
