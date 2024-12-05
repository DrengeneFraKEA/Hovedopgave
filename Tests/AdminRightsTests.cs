﻿using Hovedopgave.Server.Database;
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
            var result = await service.GetAllAdmins();

            // Assert
            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetUserByDisplayName()
        {
            // Arrange
            AdminRightsServices service = new AdminRightsServices();
            string displayName = "Quad";
            int page = 1;
            int pageSize = 5;
            // Act
            var result = await service.GetUserByDisplayName(displayName, page, pageSize);
            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetUserByDisplayName_NotFound()
        {
            // Arrange
            AdminRightsServices service = new AdminRightsServices();
            string displayName = "FakeFaker";
            int page = 1;
            int pageSize = 5;
            // Act
            var result = await service.GetUserByDisplayName(displayName, page, pageSize);
            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task SoftDeleteAndUpdateUsersRoleAndDisplayName()
        {
            // Arrange
            AdminRightsServices service = new AdminRightsServices();
            string testUserName = "TestUser";
            Roles.RoleDB newRole = Roles.RoleDB.SYSTEMADMIN;


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

            // Act 3: Verify that the user CAN NOT be updated with an existing display name 
            var updateWithAnExistingOneResult= await service.UpdateUsersDisplayName(testUserName, "Quad");

            // Assert 3: Assert that the user display name was not updated successfully
            Assert.False(updateWithAnExistingOneResult);

            // Act 4: Soft delete the user
            var softDeleteResult = await service.SoftDeleteUser(testUserName);

            // Assert 4: Assert that the user was soft deleted successfully
            Assert.True(softDeleteResult);

            // Act 5: Try to soft delete the same user again (should return false)
            var softDeleteAgainResult = await service.SoftDeleteUser(testUserName);

            // Assert 5: Assert that trying to delete an already deleted user returns false
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
