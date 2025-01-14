using Hovedopgave.Server.Controllers;
using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Tests
{
    public class AdminRightsTests
    {
        private const string ENV_FILE = "env.env";

        private readonly PostgreSQL psql;
        private readonly AdminRightsServices service;
        private readonly AdminRightsController controller;

        public AdminRightsTests()
        {
            if (File.Exists(ENV_FILE)) DotNetEnv.Env.Load(ENV_FILE);
            psql = new PostgreSQL();
            service = new AdminRightsServices();
            controller = new AdminRightsController();
        }

        [Fact]
        public async Task LoadEnvFile()
        {
            string expected = "leagues_data";
            string actual = DotNetEnv.Env.GetString("AZURE_DB_DATABASE");

            Assert.Equal(expected, actual);
        }
        // Testing service methods

        [Fact]
        public async Task GetAllAdmins()
        {
            var result = await service.GetAllAdmins();

            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetActiveUsersByDisplayName()
        {
            
            string displayName = "e";
            int page = 1;
            int pageSize = 5;
            var result = await service.SearchActiveUsers(displayName, page, pageSize);
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetSoftDeletedUsersByDisplayName()
        {

            string displayName = "e";
            int page = 1;
            int pageSize = 5;
            var result = await service.SearchDeletedUsers(displayName, page, pageSize);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateUserDetails()
        {
            string testUsername = "test_01JBC2KQ4R1KRMQCQQZ78Y34D0";

            // Insert the test user into the database
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string insertQuery = $@"INSERT INTO public.users (id, full_name, display_name, role, gender, email, password_salt, password) VALUES ('usr_01JBC2KQ4R1KRMQCQQZ78Y34D0','John Doe','{testUsername}','GUEST'::public.roles,'male','john@leagues.gg','salt','password');";
                await using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Update role
            bool updateRoleResult = await service.UpdateUsersRole("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", testUsername, Roles.Role.MODERATOR);

            // Update displayname
            UserDTO updateDetails = new UserDTO() { DisplayName = testUsername, NewDisplayName = "2KQ4R1KRMQ1JBC2KQ4R1KR4D0JBC2KQ4R1KR9", Email = "Q1JBC2KQ4R1KR4D0JBC2K@blabla.com", FullName = "John Smith" };
            
            bool updateDisplayNameResult = await service.UpdateUserDetails("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", updateDetails);

            // Hard delete user
            var softDeleteResult = await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", updateDetails.NewDisplayName);

            Assert.True(updateRoleResult && updateDisplayNameResult && softDeleteResult);

            // Clean up
            await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", updateDetails.NewDisplayName);
        }

        [Fact]
        public async Task SoftDeleteUser() 
        {
            string testUsername = "test_C2KQ4R1KRMQCQQZ78Y34D0ASDF";
            string testUserId = "usr_01JBC2KQ4R1KRMQCQQZ78Y34D0";

            // Insert the test user into the database
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string insertQuery = $@"INSERT INTO public.users (id, full_name, display_name, role, gender, email, password_salt, password) VALUES ('{testUserId}','John Doe','{testUsername}','GUEST'::public.roles,'male','john@leagues.gg','salt','password');";
                await using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            Users user = await service.GetUserById(testUserId);

            Assert.True(user.display_name != null);

            bool deleted = await service.SoftDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", user.display_name);

            Assert.True(deleted);

            bool hDeleted = await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", user.display_name);

            Assert.True(hDeleted);

            // Clean up
            await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", user.display_name);
        }


        // Testing controller methods

        [Fact]
        public async Task GetAdminsController()
        {
            // Act
            var result = await controller.GetAdmins();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotEmpty((List<UserDTO>)okResult.Value);
        }

        [Fact]
        public async Task SearchActiveUsersController()
        {
            // Arrange
            string displayName = "e";
            int page = 1;
            int pageSize = 5;

            // Act
            var result = await controller.SearchActiveUsers(displayName, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetDeletedUsersController()
        {
            // Arrange
            string displayName = "e";
            int page = 1;
            int pageSize = 5;

            // Act
            var result = await controller.GetDeletedUsers(displayName, page, pageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task SoftDeleteUserController()
        {
            // Arrange
            string testUsername = "test_C2KQ4R1KRMQCQQZ78Y34D0ASDF";
            string testUserId = "usr_01JBC2KQ4R1KRMQCQQZ78Y34D0";
            var loggedInUser = new LoggedInUser { LoggedInUserID = "usr_01JBC2KQ4SAV6SW2QDC6DKSKD7" };

            // Insert the test user into the database
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string insertQuery = $@"INSERT INTO public.users (id, full_name, display_name, role, gender, email, password_salt, password) VALUES ('{testUserId}','John Doe','{testUsername}','GUEST'::public.roles,'male','john@leagues.gg','salt','password');";
                await using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Act
            var result = await controller.SoftDeleteUser(testUsername, loggedInUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Clean up
            await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", testUsername);
        }

        [Fact]
        public async Task UpdateUsersRoleController()
        {
            // Arrange
            string testUsername = "test_01JBC2KQ4R1KRMQCQQZ78Y34D0";
            var loggedInUser = new LoggedInUser { LoggedInUserID = "usr_01JBC2KQ4SAV6SW2QDC6DKSKD7" };

            // Insert the test user into the database
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string insertQuery = $@"INSERT INTO public.users (id, full_name, display_name, role, gender, email, password_salt, password) VALUES ('usr_01JBC2KQ4R1KRMQCQQZ78Y34D0','John Doe','{testUsername}','GUEST'::public.roles,'male','john@leagues.gg','salt','password');";
                await using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Act
            var result = await controller.UpdateUsersRole(testUsername, Roles.Role.MODERATOR, loggedInUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Clean up
            await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", testUsername);
        }


        [Fact]
        public async Task UpdateUserDetailsController()
        {
            // Arrange
            string testUsername = "test_01JBC2KQ4R1KRMQCQQZ78Y34D0";
            var loggedInUser = new LoggedInUser { LoggedInUserID = "usr_01JBC2KQ4SAV6SW2QDC6DKSKD7" };
            UserDTO updateDetails = new UserDTO() { DisplayName = testUsername, NewDisplayName = "2KQ4R1KRMQ1JBC2KQ4R1KR4D0JBC2KQ4R1KR9", Email = "Q1JBC2KQ4R1KR4D0JBC2K@blabla.com", FullName = "John Smith", LoggedInUser = loggedInUser.LoggedInUserID };

            // Insert the test user into the database
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string insertQuery = $@"INSERT INTO public.users (id, full_name, display_name, role, gender, email, password_salt, password) VALUES ('usr_01JBC2KQ4R1KRMQCQQZ78Y34D0','John Doe','{testUsername}','GUEST'::public.roles,'male','john@leagues.gg','salt','password');";
                await using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Act
            var result = await controller.UpdateUserDetails(testUsername, updateDetails);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Clean up
            await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", updateDetails.NewDisplayName);
        }

        [Fact]
        public async Task UndeleteUserController()
        {
            // Arrange
            string testUsername = "test_01JBC2KQ4R1KRMQCQQZ78Y34D0";
            var loggedInUser = new LoggedInUser { LoggedInUserID = "usr_01JBC2KQ4SAV6SW2QDC6DKSKD7" };

            // Insert the test user into the database
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string insertQuery = $@"INSERT INTO public.users (id, full_name, display_name, role, gender, email, password_salt, password) VALUES ('usr_01JBC2KQ4R1KRMQCQQZ78Y34D0','John Doe','{testUsername}','GUEST'::public.roles,'male','john@leagues.gg','salt','password');";
                await using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Soft delete user
            await service.SoftDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", testUsername);

            // Act
            var result = await controller.UndeleteUser(testUsername, loggedInUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Clean up
            await service.HardDeleteUser("usr_01JBC2KQ4SAV6SW2QDC6DKSKD7", testUsername);
        }

    }
}
