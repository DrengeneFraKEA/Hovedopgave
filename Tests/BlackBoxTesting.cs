using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Services;
using Hovedopgave.Server.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class BlackBoxTesting
    {

        private const string ENV_FILE = "env.env";

        PostgreSQL psql;

        public BlackBoxTesting()
        {
            if (File.Exists(ENV_FILE)) DotNetEnv.Env.Load(ENV_FILE);
            psql = new PostgreSQL();
        }

        [Theory]
        // Boundary value analysis for username and password length
        [InlineData("", "", "no credentials provided.")]
        [InlineData("a", "a", "no user found.")]
        [InlineData("nB0007MzJbMiQ5QwZBP1zqfBiDOoZvLuITchgUB2FCViyhBiDsgvKhytPiOP2K16h7s909xbEl11LTgJ5q9vhsEvnGrC1VSGDmRL", "nB0007MzJbMiQ5QwZBP1zqfBiDOoZvLuITchgUB2FCViyhBiDsgvKhytPiOP2K16h7s909xbEl11LTgJ5q9vhsEvnGrC1VSGDmRL", "no user found.")]
        [InlineData("nB0007MzJbMiQ5QwZBP1zqfBiDOoZvLuITchgUB2FCViyhBiDsgvKhytPiOP2K16h7s909xbEl11LTgJ5q9vhsEvnGrC1VSGDmRL1", "password", "invalid input provided.")]
        [InlineData("username", "nB0007MzJbMiQ5QwZBP1zqfBiDOoZvLuITchgUB2FCViyhBiDsgvKhytPiOP2K16h7s909xbEl11LTgJ5q9vhsEvnGrC1VSGDmRL1", "invalid input provided.")]
        // Equivalence partitioning for valid and invalid inputs
        [InlineData("validUsername", "validPassword", "no user found.")]
        [InlineData("invalidUsername!", "validPassword", "invalid input provided.")]
        [InlineData("validUsername", "invalidPassword!", "invalid input provided.")]
        public async Task LoginTest(string username, string password, string expectedError)
        {
            // Arrange
            var loginService = new LoginServices();
            var credentials = new LoginAttemptDTO { username = username, password = password };

            // Act
            var result = await loginService.Login(credentials);

            // Assert
            Assert.Equal(expectedError, result.error);
        }

        [Theory]
        // Boundary value analysis & Equivalence partitioning for display name
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

        [Theory]
        // Boundary value analysis & Equivalence partitioning for display name
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
    }
}
