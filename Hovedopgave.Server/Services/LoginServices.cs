using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Utils;
using Npgsql;

namespace Hovedopgave.Server.Services
{
    public class LoginServices
    {
        public async Task<LoginDTO> Login(LoginAttemptDTO credentials) 
        {
            if (string.IsNullOrEmpty(credentials.username) || string.IsNullOrEmpty(credentials.password)) return null;

            PostgreSQL psql = new PostgreSQL(true); // change to false once azure is up
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT * FROM users WHERE display_name='{credentials.username}'");
            await using var reader = await command.ExecuteReaderAsync();

            Users tempUser = null;

            // Retrieve user by username/display_name
            while (await reader.ReadAsync())
            {
                tempUser = new Users()
                {
                    id = reader.GetString(0),
                    display_name = reader.GetString(2),
                    role = Roles.GetRoleByName(reader.GetString(3)),
                    password_salt = reader.GetString(6),
                    password = reader.GetString(7),
                };
            }

            // Take the saved salted password and salt the provided password in the login attempt, then hash it and compare if it is the same.
            if (tempUser != null) 
            {
                string hashedPasswordAttempt = PasswordHandler.GetHashedPassword(credentials.password, tempUser.password_salt);

                if (hashedPasswordAttempt == tempUser.password && tempUser.role == Roles.Role.SYSTEMADMIN || tempUser.role == Roles.Role.SUPERUSER || tempUser.role == Roles.Role.MODERATOR)
                {
                    JwtTokenGenerator jwt = new JwtTokenGenerator();

                    LoginDTO loginDto = new LoginDTO()
                    {
                        username = tempUser.display_name,
                        user_id = tempUser.id,
                        token = jwt.GenerateToken(tempUser.id, tempUser.role.ToString())
                    };

                    return loginDto;
                }
            }

            return null;
        }
    }
}
