using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Services;
using Npgsql;

namespace Tests
{
    public class AdminRightsTests
    {
        private const string ENV_FILE = "env.env";

        PostgreSQL psql;

        public AdminRightsTests()
        {
            if (File.Exists(ENV_FILE)) DotNetEnv.Env.Load(ENV_FILE);
            psql = new PostgreSQL();
        }

        [Fact]
        public async Task LoadEnvFile()
        {
            string expected = "leagues_data";
            string actual = DotNetEnv.Env.GetString("AZURE_DB_DATABASE");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task GetAllAdmins()
        {
            AdminRightsServices service = new AdminRightsServices();
            var result = await service.GetAllAdmins();

            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetActiveUsersByDisplayName()
        {
            AdminRightsServices service = new AdminRightsServices();

            string displayName = "e";
            int page = 1;
            int pageSize = 5;
            var result = await service.SearchActiveUsers(displayName, page, pageSize);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetSoftDeletedUsersByDisplayName()
        {
            AdminRightsServices service = new AdminRightsServices();

            string displayName = "e";
            int page = 1;
            int pageSize = 5;
            var result = await service.SearchDeletedUsers(displayName, page, pageSize);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateUserDetails()
        {
            AdminRightsServices service = new AdminRightsServices();
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
        }

        [Fact]
        public async Task SoftDeleteUser()
        {
            AdminRightsServices service = new AdminRightsServices();
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
        }


        [Theory]
        [InlineData("", 1, 10, "display name cannot be empty.")]
        [InlineData("a", 1, 10, null)]
        [InlineData("deletedUser456", 1, 10, null)]
        [InlineData("deletedUserWithInvalidChar!", 1, 10, "invalid characters in display name.")]
        [InlineData("deletedUserWithVeryLongDisplayNameExceedingTheMaximumAllowedCharacterLimitOfOneHundredCharactersXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", 1, 10, "display name cannot exceed 100 characters.")]
        public async Task SearchDeletedUsersTest(string displayName, int page, int pageSize, string expectedError)
        {
            // Arrange
            var adminService = new AdminRightsServices();

            // Act & Assert
            if (!string.IsNullOrEmpty(expectedError))
            {
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => adminService.SearchDeletedUsers(displayName, page, pageSize));
                Assert.Equal(expectedError, exception.Message);
            }
            else
            {
                var result = await adminService.SearchDeletedUsers(displayName, page, pageSize);
                Assert.NotNull(result);
            }
        }

        [Theory]
        [InlineData("", 1, 10, "display name cannot be empty.")]
        [InlineData("a", 1, 10, null)]
        [InlineData("validUser123", 1, 10, null)]
        [InlineData("userWithInvalidChar!", 1, 10, "invalid characters in display name.")]
        [InlineData("userWithVeryLongDisplayNameExceedingTheMaximumAllowedCharacterLimitOfOneHundredCharactersXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", 1, 10, "display name cannot exceed 100 characters.")]

        public async Task SearchActiveUsersTest(string displayName, int page, int pageSize, string expectedError)
        {
            // Arrange
            var adminService = new AdminRightsServices();

            // Act & Assert
            if (!string.IsNullOrEmpty(expectedError))
            {
                var exception = await Assert.ThrowsAsync<ArgumentException>(() => adminService.SearchActiveUsers(displayName, page, pageSize));
                Assert.Equal(expectedError, exception.Message);
            }
            else
            {
                var result = await adminService.SearchActiveUsers(displayName, page, pageSize);
                Assert.NotNull(result);
            }
        }
    }
}