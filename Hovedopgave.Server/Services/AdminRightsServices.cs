using Hovedopgave.Server.Database;
using Npgsql;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;

namespace Hovedopgave.Server.Services
{
    public class AdminRightsServices
    {
        public async Task<List<GetAllUsersDTO>> GetAllUsers()
        {
            PostgreSQL psql = new PostgreSQL(true); // Change to false once Azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            List<GetAllUsersDTO> users = new List<GetAllUsersDTO>();

            // Query to fetch only the display_name and role columns
            await using var command = conn.CreateCommand("SELECT display_name, role FROM public.users where deleted_at is null");
            await using var reader = await command.ExecuteReaderAsync();

            // Iterate through the results and populate the list
            while (await reader.ReadAsync())
            {
                users.Add(new GetAllUsersDTO
                {
                    DisplayName = reader.GetString(0),
                    Role = reader.GetString(1),
                });
            }

            return users;
        }


        public async Task<GetAllUsersDTO?> GetUserByDisplayName(string displayName)
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
                return new GetAllUsersDTO
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

            // Query to update display name
            await using var command = conn.CreateCommand(
                "UPDATE public.users SET display_name = @newDisplayName WHERE display_name = @displayName AND deleted_at IS NULL"
            );

            // Add parameters
            command.Parameters.AddWithValue("displayName", displayName);
            command.Parameters.AddWithValue("newDisplayName", newDisplayName);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if a row was updated

        }

    }
}
