using Hovedopgave.Server.Database;
using Npgsql;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Utils;
using System.Text;
using Newtonsoft.Json;

namespace Hovedopgave.Server.Services
{
    public class AdminRightsServices
    {
        public async Task<List<UserDTO>> GetAllAdmins()
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            List<UserDTO> admins = new List<UserDTO>();

            // Query to fetch only the display_name and role columns
            await using var command = conn.CreateCommand("SELECT display_name, role FROM public.users where deleted_at is null AND role in ('SYSTEMADMIN', 'SUPERUSER')");
            await using var reader = await command.ExecuteReaderAsync();

            // Iterate through the results and populate the list
            while (await reader.ReadAsync())
            {
                admins.Add(new UserDTO
                {
                    DisplayName = reader.GetString(0),
                    Role = reader.GetString(1),
                });
            }

            return admins;
        }


        public async Task<List<UserDTO?>> GetUserByDisplayName(string displayName, int page, int pageSize)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            List<UserDTO> users = new List<UserDTO>();

            // Query to fetch user by display_name with pagination
            await using var command = conn.CreateCommand(
                "SELECT display_name, role FROM public.users WHERE deleted_at IS NULL AND display_name ILIKE @displayName LIMIT @pageSize OFFSET @offset"
            );

            // Add the displayName parameter
            command.Parameters.AddWithValue("displayName", "%" + displayName + "%");
            command.Parameters.AddWithValue("pageSize", pageSize);
            command.Parameters.AddWithValue("offset", (page - 1) * pageSize);

            await using var reader = await command.ExecuteReaderAsync();

            // Iterate through the results and populate the list 
            while (await reader.ReadAsync())
            {
                users.Add(new UserDTO
                {
                    DisplayName = reader.GetString(0),
                    Role = reader.GetString(1),
                });
            }

            return users;
        }

        private async Task<Roles.Role> GetUserRoleByDisplayName(string displayName, NpgsqlDataSource conn)
        {
            await using var command = conn.CreateCommand(
                "SELECT role FROM public.users WHERE display_name = @displayName AND deleted_at IS NULL"
            );
            command.Parameters.AddWithValue("displayName", displayName);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Roles.GetRoleByName(reader.GetString(0));
            }

            return Roles.Role.GUEST;
        }

        public async Task<bool> SoftDeleteUser(string loggedInUserDisplayName, string displayName)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            DateTime timestamp = DateTime.UtcNow;

            // Get logged in users role
            Roles.Role loggedInUserRole = await GetUserRoleByDisplayName(loggedInUserDisplayName, conn);

            // Get target user's role
            Roles.Role targetUserRole = await GetUserRoleByDisplayName(displayName, conn);

            // Check if logged in user can change the target users role
            if (!Roles.CanChangeRole(loggedInUserRole, targetUserRole))
            {
                return false; // Not high enough privileges/access level
            }

            // Query to update deleted_at 
            await using var command = conn.CreateCommand(
                "UPDATE public.users SET deleted_at = @timestamp WHERE display_name = @displayName AND deleted_at IS NULL"
            );

            // Add parameters
            command.Parameters.AddWithValue("displayName", displayName);
            command.Parameters.AddWithValue("timestamp", timestamp);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if a row was updated

        }

        public async Task<bool> UpdateUsersRole(string loggedInUserDisplayName, string displayName, Roles.Role role)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Get logged in users role
            Roles.Role loggedInUserRole = await GetUserRoleByDisplayName(loggedInUserDisplayName, conn);

            // Get target users role
            Roles.Role targetUserRole = await GetUserRoleByDisplayName(displayName, conn);

            // Check if logged in user can change the target users role
            if (!Roles.CanChangeRole(loggedInUserRole, targetUserRole))
            {
                return false; // Not high enough privileges/access level
            }

            // Check if logged in user can change to this role
            if (!Roles.CanChangeRole(loggedInUserRole, role))
            {
                return false; // Not high enough privileges/access level
            }

            // Query to update role
            await using var command = conn.CreateCommand(
                "UPDATE public.users SET role = @newRole::public.roles WHERE display_name = @displayName AND deleted_at IS NULL"
            );

            // Add parameters
            command.Parameters.AddWithValue("displayName", displayName);
            command.Parameters.AddWithValue("newRole", role.ToString());

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if a row was updated

        }

        public async Task<bool> UpdateUsersDisplayName(string loggedInUserDisplayName, string displayName, string newDisplayName)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Get logged in users role
            Roles.Role loggedInUserRole = await GetUserRoleByDisplayName(loggedInUserDisplayName, conn);

            // Get target users role
            Roles.Role targetUserRole = await GetUserRoleByDisplayName(displayName, conn);

            // Check if logged in user can change the target users role
            if (!Roles.CanChangeRole(loggedInUserRole, targetUserRole))
            {
                return false; // Not high enough privileges/access level
            }

            // Checks if the new displa name already exists
            await using var checkCommand = conn.CreateCommand("SELECT COUNT(*) FROM public.users WHERE display_name = @newDisplayName");
            checkCommand.Parameters.AddWithValue("newDisplayName", newDisplayName);

            int count = (int)(long) await checkCommand.ExecuteScalarAsync();
            if (count > 0)
            {
                return false; // Returns false if the new display name already exists
            }

            // Query to update display name
            await using var updateCommand = conn.CreateCommand(
                "UPDATE public.users SET display_name = @newDisplayName WHERE display_name = @displayName AND deleted_at IS NULL"
            );

            // Add parameters
            updateCommand.Parameters.AddWithValue("displayName", displayName);
            updateCommand.Parameters.AddWithValue("newDisplayName", newDisplayName);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if a row was updated

        }

        public async Task<bool> ResetUserPassword(string displayName)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Get user's email
            await using var command = conn.CreateCommand(
                "SELECT email FROM public.users WHERE display_name = @displayName AND deleted_at IS NULL"
            );
            command.Parameters.AddWithValue("displayName", displayName);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return false; // User not found
            }

            string email = reader.GetString(0);

            // Generate new password
            string newPassword = PasswordHandler.GenerateRandomPassword();
            string salt = PasswordHandler.GenerateSalt();
            string hashedPassword = PasswordHandler.GetHashedPassword(newPassword, salt);

            // Update users password in the database
            await using var updateCommand = conn.CreateCommand(
                "UPDATE public.users SET password = @hashedPassword, password_salt = @salt WHERE display_name = @displayName AND deleted_at IS NULL"
            );
            updateCommand.Parameters.AddWithValue("hashedPassword", hashedPassword);
            updateCommand.Parameters.AddWithValue("salt", salt);
            updateCommand.Parameters.AddWithValue("displayName", displayName);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                return false; // Failed to update password
            }

            // Send email with new password
            await SendPasswordResetEmail(email, newPassword);

            return true;
        }

        private async Task SendPasswordResetEmail(string email, string newPassword)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    to = email,
                    subject = "Password Reset",
                    text = $"Your new password is: {newPassword}"
                }), Encoding.UTF8, "application/json");

                await client.PostAsync("http://localhost:3000/send-email", content);
            }
        }

    }
}
