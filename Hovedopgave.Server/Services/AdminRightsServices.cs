using Hovedopgave.Server.Database;
using Npgsql;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;

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
            await using var command = conn.CreateCommand("SELECT display_name, role FROM public.users where deleted_at is null AND role = 'admin'");
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


        public async Task<UserDTO?> GetUserByDisplayName(string displayName)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Query to fetch user by display_name
            await using var command = conn.CreateCommand(
                "SELECT display_name, role FROM public.users WHERE deleted_at IS NULL AND display_name = @displayName"
            );

            // Add the displayName parameter
            command.Parameters.AddWithValue("displayName", displayName);

            await using var reader = await command.ExecuteReaderAsync();

            // Check if a user is found
            if (await reader.ReadAsync())
            {
                return new UserDTO
                {
                    DisplayName = reader.GetString(0),
                    Role = reader.GetString(1),
                };
            }

            return null;
        }


        public async Task<bool> SoftDeleteUser(string displayName)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            DateTime timestamp = DateTime.UtcNow;

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

        public async Task<bool> UpdateUsersRole(string displayName, Roles.RoleDB role)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

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

        public async Task<bool> UpdateUsersDisplayName(string displayName, string newDisplayName)
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

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

    }
}
