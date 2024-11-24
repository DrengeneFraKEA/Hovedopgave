using Hovedopgave.Server.Database;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Services;
using Npgsql;

namespace Tests
{
    public class AdminRightsTests
    {
        PostgreSQL psql = new PostgreSQL(true);


        [Fact]
        public async Task GetAllUsers()
        {
            // Act
            AdminRightsServices service = new AdminRightsServices();
            var result = await service.GetAllUsers();

            // Assert
            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetUserByDisplayName()
        {
            // Arrange
            AdminRightsServices service = new AdminRightsServices();
            string displayName = "Quad";
            // Act
            var result = await service.GetUserByDisplayName(displayName);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserByDisplayName_NotFound()
        {
            // Arrange
            AdminRightsServices service = new AdminRightsServices();
            string displayName = "FakeFaker";
            // Act
            var result = await service.GetUserByDisplayName(displayName);
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteAndUpdateUsersRole()
        {
            // Arrange
            AdminRightsServices service = new AdminRightsServices();
            string testUserName = "TestUser";
            Roles.RoleDB newRole = Roles.RoleDB.admin;


            // Step 1: Insert the test user into the database
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string insertQuery = $@"
                    INSERT INTO public.users (id, full_name, display_name, role, gender, email, password_salt, password)
                    VALUES
                    ('usr_01JBC2KQ4R1KRMQCQQZ78Y34D0','John Doe','{testUserName}','user'::public.roles,'male','john@leagues.gg','salt','password');
                ";
                await using (var cmd = new NpgsqlCommand(insertQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Act 1: Verify that the user can be updated (change role)
            var updateRoleResult = await service.UpdateUsersRole(testUserName, newRole);

            // Assert 1: Assert that the user role was updated successfully
            Assert.True(updateRoleResult);

            // Act 2: Verify that the user can be updated (change display name)
            var updateDisplayNameResult = await service.UpdateUsersDisplayName(testUserName, "TestUserChangedName");

            // Assert 2: Assert that the user display name was updated successfully
            Assert.True(updateDisplayNameResult);
            testUserName = "TestUserChangedName";
            // Act 3: Soft delete the user
            var softDeleteResult = await service.SoftDeleteUser(testUserName);

            // Assert 3: Assert that the user was soft deleted successfully
            Assert.True(softDeleteResult);

            // Act 4: Try to soft delete the same user again (should return false)
            var softDeleteAgainResult = await service.SoftDeleteUser(testUserName);

            // Assert 4: Assert that trying to delete an already deleted user returns false
            Assert.False(softDeleteAgainResult);

            // Step 2: Delete the test user from the database to clean up
            using (var connection = new NpgsqlConnection(psql.connectionstring))
            {
                await connection.OpenAsync();
                string deleteQuery = $@"
                    DELETE FROM public.users WHERE display_name = '{testUserName}';
                ";
                await using (var cmd = new NpgsqlCommand(deleteQuery, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
