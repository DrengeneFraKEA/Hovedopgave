using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class LoginTests
    {
        private const string ENV_FILE = "env.env";

        PostgreSQL psql;

        public LoginTests()
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
    }
}
