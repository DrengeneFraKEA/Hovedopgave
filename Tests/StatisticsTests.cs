﻿using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class StatisticsTests
    {
        private const string ENV_FILE = "env.env";

        PostgreSQL psql;

        public StatisticsTests()
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
        public async Task GetDailyUsers()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetDailyUsers(30);

            Assert.NotEmpty(result);
        }


        [Fact]
        public async Task GetMonthlyUsers()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetMonthlyUsers(12);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetDailyTeams()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetDailyTeams(30);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetMonthlyTeams()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetMonthlyTeams(12);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetDailyOrganisations()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetDailyOrganisations(30);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetMonthlyOrganisations()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetMonthlyOrganisations(12);

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetCustomGraphUsers()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetCustomGraphData("10/10/2023", "11/11/2024", "users");

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetCustomGraphTeams()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetCustomGraphData("10/10/2023", "11/11/2024", "teams");

            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetCustomGraphOrganisations()
        {
            GraphService service = new GraphService();
            GraphDTO[] result = await service.GetCustomGraphData("10/10/2023", "11/11/2024", "organisations");

            Assert.NotEmpty(result);
        }
    }
}
