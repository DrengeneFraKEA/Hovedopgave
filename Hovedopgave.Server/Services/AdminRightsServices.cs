﻿using Hovedopgave.Server.Database;
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
            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            List<UserDTO> admins = new List<UserDTO>();

            // Query to fetch only the display_name and role columns
            await using var command = conn.CreateCommand("SELECT display_name, role, full_name, email FROM public.users where deleted_at is null AND role in ('SYSTEMADMIN', 'SUPERUSER')");
            await using var reader = await command.ExecuteReaderAsync();

            // Iterate through the results and populate the list
            while (await reader.ReadAsync())
            {
                admins.Add(new UserDTO
                {
                    DisplayName = reader.GetString(0),
                    Role = reader.GetString(1),
                    FullName = reader.GetString(2),
                    Email = reader.GetString(3)
                });
            }

            return admins;
        }


        public async Task<List<UserDTO>> SearchActiveUsers(string displayName, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException("display name cannot be empty.");
            }
            if (!Sanitizer.CheckInputValidity(displayName))
            {
                throw new ArgumentException("invalid characters in display name.");
            }
            if (displayName.Length > 100)
            {
                throw new ArgumentException("display name cannot exceed 100 characters.");
            }

            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            List<UserDTO> users = new List<UserDTO>();

            string query = @"
            SELECT display_name, role, full_name, email
            FROM public.users
            WHERE deleted_at IS NULL AND display_name ILIKE @displayName
            order by display_name asc
            LIMIT @pageSize OFFSET @offset";

            await using var command = conn.CreateCommand(query);

            command.Parameters.AddWithValue("displayName", "%" + displayName + "%");
            command.Parameters.AddWithValue("pageSize", pageSize);
            command.Parameters.AddWithValue("offset", (page - 1) * pageSize);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new UserDTO
                {
                    DisplayName = reader.GetString(0),
                    Role = reader.GetString(1),
                    FullName = reader.GetString(2),
                    Email = reader.GetString(3)
                });
            }

            return users;
        }

        public async Task<List<UserDTO>> SearchDeletedUsers(string displayName, int page, int pageSize)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                throw new ArgumentException("display name cannot be empty.");
            }
            if (!Sanitizer.CheckInputValidity(displayName))
            {
                throw new ArgumentException("invalid characters in display name.");
            }
            if (displayName.Length > 100)
            {
                throw new ArgumentException("display name cannot exceed 100 characters.");
            }

            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            List<UserDTO> users = new List<UserDTO>();

            string query = $"SELECT display_name, role, full_name, email FROM public.users WHERE deleted_at IS NOT NULL AND display_name ILIKE '%{displayName}%' order by display_name asc LIMIT '{page}' OFFSET '{(page - 1) * pageSize}'";

            await using var command = conn.CreateCommand(query);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                users.Add(new UserDTO
                {
                    DisplayName = reader.GetString(0),
                    Role = reader.GetString(1),
                    FullName = reader.GetString(2),
                    Email = reader.GetString(3)
                });
            }

            return users;
        }

        private async Task<Roles.Role> GetUserRoleByDisplayName(string displayName, NpgsqlDataSource conn, bool includeDeleted = false)
        {
            
            string query = "SELECT role FROM public.users WHERE display_name = @displayName";
            
            if (!includeDeleted)
            {
                query += " AND deleted_at IS NULL";
            }

            await using var command = conn.CreateCommand(query);
            command.Parameters.AddWithValue("displayName", displayName);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Roles.GetRoleByName(reader.GetString(0));
            }

            return Roles.Role.INVALID;
        }

        private async Task<Roles.Role> GetUserRoleByID(string id, NpgsqlDataSource conn)
        { 

            string query = "SELECT role FROM public.users WHERE id = @id";

            await using var command = conn.CreateCommand(query);
            command.Parameters.AddWithValue("id", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return Roles.GetRoleByName(reader.GetString(0));
            }

            return Roles.Role.GUEST;
        }

        public async Task<bool> SoftDeleteUser(string loggedInUserID, string displayName)
        {
            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            DateTime timestamp = DateTime.UtcNow;

            // Get logged in users role
            Roles.Role loggedInUserRole = await GetUserRoleByID(loggedInUserID, conn);

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

        public async Task<bool> UpdateUsersRole(string loggedInUserID, string displayName, Roles.Role role)
        {
            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Get logged in users role
            Roles.Role loggedInUserRole = await GetUserRoleByID(loggedInUserID, conn);

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

        public async Task<bool> UpdateUserDetails(string loggedInUserID, UserDTO user)
        {
            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Get logged in users role
            Roles.Role loggedInUserRole = await GetUserRoleByID(loggedInUserID, conn);

            // Get target users role
            Roles.Role targetUserRole = await GetUserRoleByDisplayName(user.DisplayName, conn);

            // Check if logged in user can change the target users role
            if (!Roles.CanChangeRole(loggedInUserRole, targetUserRole))
            {
                return false; // Not high enough privileges/access level
            }
            if (user.NewDisplayName != user.DisplayName) {
                // Checks if the new display name already exists
                await using var checkCommand = conn.CreateCommand("SELECT COUNT(*) FROM public.users WHERE display_name = @newDisplayName");
                checkCommand.Parameters.AddWithValue("newDisplayName", user.NewDisplayName);

                int count = (int)(long)await checkCommand.ExecuteScalarAsync();
                if (count > 0)
                {
                    return false; // Returns false if the new display name already exists
                }
            }
            // Query to update details
            await using var updateCommand = conn.CreateCommand(
                "UPDATE public.users SET display_name = @newDisplayName, full_name = @fullName, email = @email WHERE display_name = @displayName AND deleted_at IS NULL"
            );

            // Add parameters
            updateCommand.Parameters.AddWithValue("displayName", user.DisplayName);
            updateCommand.Parameters.AddWithValue("newDisplayName", user.NewDisplayName);
            updateCommand.Parameters.AddWithValue("fullName", user.FullName);
            updateCommand.Parameters.AddWithValue("email", user.Email);

            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if a row was updated

        }

        public async Task<bool> ResetUserPassword(string displayName)
        {
            PostgreSQL psql = new PostgreSQL();
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

                await client.PostAsync("http://localhost:8080/send-email", content);
            }
        }

        public async Task<bool> HardDeleteUser(string loggedInUserID, string targetDisplayName)
        {
            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Get logged in users role
            Roles.Role loggedInUserRole = await GetUserRoleByID(loggedInUserID, conn);

            // Check if the logged in user is SYSTEMADMIN
            if (loggedInUserRole != Roles.Role.SYSTEMADMIN)
            {
                return false; // Not enough privileges
            }

            // Hard delete user
            await using var command = conn.CreateCommand(
                "DELETE FROM public.users WHERE display_name = @displayName"
            );

            // Add parameters
            command.Parameters.AddWithValue("displayName", targetDisplayName);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if a row was deleted
        }

        public async Task<bool> UndeleteUser(string loggedInUserID, string displayName)
        {
            PostgreSQL psql = new PostgreSQL();
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            // Get logged in user's role
            Roles.Role loggedInUserRole = await GetUserRoleByID(loggedInUserID, conn);

            // Get target user's role
            Roles.Role targetUserRole = await GetUserRoleByDisplayName(displayName, conn, includeDeleted: true);

            // Check if logged in user can change the target user's role
            if (!Roles.CanChangeRole(loggedInUserRole, targetUserRole))
            {
                return false; // Not enough privileges
            }

            // Update deleted_at to NULL
            await using var command = conn.CreateCommand(
                "UPDATE public.users SET deleted_at = NULL WHERE display_name = @displayName AND deleted_at IS NOT NULL"
            );
            command.Parameters.AddWithValue("displayName", displayName);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if a row was updated
        }

        public async Task<Users> GetUserById(string id) 
        {
            PostgreSQL psql = new PostgreSQL();

            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);
            await using var command = conn.CreateCommand($"SELECT display_name FROM public.users WHERE id = '{id}'");

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync()) return new Users() { display_name = reader.GetString(0) };
            else return null;
        }
    }
}
