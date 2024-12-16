using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Npgsql;

namespace Hovedopgave.Server.Services
{
    public class GraphService : IGraphService
    {
        public async Task<GraphDTO[]> GetCustomUsers(string fromDate, string toDate)
        {
            DateTime from = DateTime.Parse(fromDate);
            DateTime to = DateTime.Parse(toDate);

            if (from > to || from == to) return new GraphDTO[] { }; // Jump out

            // Get days in between
            int daysInBetween = (to - from).Days;

            GraphDTO[] gdto = new GraphDTO[0];

            // Get relevant graph format.. Days, weeks, months, years..
            CustomGraphFormats.CustomGraphFormat graphFormat = CustomGraphFormats.GetGraphFormat(daysInBetween);

            switch (graphFormat) 
            {
                case CustomGraphFormats.CustomGraphFormat.Days:
                    gdto = new GraphDTO[daysInBetween + 1];
                    for (int i = 0; i <= daysInBetween; i++) gdto[i] = new GraphDTO() {date = from.AddDays(i).Date.ToString("dd/MM/yyyy"), value = 0 };
                    break;
                case CustomGraphFormats.CustomGraphFormat.Weeks:
                    // Divide amount of days in between the provided date with 7 to get the amount of weeks, then ceil it and make that the size of the graphdto.
                    double weeks = daysInBetween / 7.0;
                    decimal actualWeeks = Math.Ceiling((decimal)weeks);
                    gdto = new GraphDTO[(int)actualWeeks];

                    for (int i = 0; i < actualWeeks; i++)
                    {
                        gdto[i] = new GraphDTO() { date = from.AddDays(i * 7).Date.ToString("dd/MM/yyyy"), value = 0 };

                        // Make sure the remaining days is no longer than a week, so that the 'daysinbetween' array can be set to the correct size and not go over the toDate.
                        DateTime parsedDate = DateTime.Parse(gdto[i].date);
                        int remainingDays = (to - parsedDate).Days;

                        if (remainingDays > 7) gdto[i].daysInBetween = new string[6];
                        else gdto[i].daysInBetween = new string[remainingDays];

                        DateTime date = DateTime.Parse(gdto[i].date);
                        for (int k = 1; k <= gdto[i].daysInBetween.Length; k++)
                        {
                            gdto[i].daysInBetween[k - 1] = date.AddDays(k).Date.ToString("dd/MM/yyyy");
                        }
                    }
                    break;
                case CustomGraphFormats.CustomGraphFormat.Months:
                    // Divide with 30 to get months and ceil.
                    double months = daysInBetween / 30.0;
                    decimal actualMonths = Math.Ceiling((decimal)months);

                    gdto = new GraphDTO[(int)actualMonths];

                    for (int i = 0; i < actualMonths; i++)
                    {
                        gdto[i] = new GraphDTO() { date = from.AddDays(i * 30).Date.ToString("dd/MM/yyyy"), value = 0 };

                        DateTime parsedDate = DateTime.Parse(gdto[i].date);
                        int remainingDays = (to - parsedDate).Days;

                        if (remainingDays > 30) gdto[i].daysInBetween = new string[29];
                        else gdto[i].daysInBetween = new string[remainingDays];

                        DateTime date = DateTime.Parse(gdto[i].date);

                        for (int k = 1; k <= gdto[i].daysInBetween.Length; k++)
                        {
                            gdto[i].daysInBetween[k - 1] = date.AddDays(k).Date.ToString("dd/MM/yyyy");
                        }
                    }

                    break;
                case CustomGraphFormats.CustomGraphFormat.Years:
                    // Same drill as months and weeks
                    double years = daysInBetween / 365.0;
                    decimal actualYears = Math.Ceiling((decimal)years);

                    gdto = new GraphDTO[(int)actualYears];

                    for (int i = 0; i < actualYears; i++)
                    {
                        gdto[i] = new GraphDTO() { date = from.AddDays(i * 365).Date.ToString("dd/MM/yyyy"), value = 0 };

                        // Make sure the remaining days is no longer than a year, so that the 'daysinbetween' array can be set to the correct size and not go over the toDate.
                        DateTime parsedDate = DateTime.Parse(gdto[i].date);
                        int remainingDays = (to - parsedDate).Days;

                        if (remainingDays > 365) gdto[i].daysInBetween = new string[364];
                        else gdto[i].daysInBetween = new string[remainingDays];

                        DateTime date = DateTime.Parse(gdto[i].date);

                        for (int k = 1; k <= gdto[i].daysInBetween.Length; k++)
                        {
                            gdto[i].daysInBetween[k - 1] = date.AddDays(k).Date.ToString("dd/MM/yyyy");
                        }
                    }
                    break;
            }

            string desiredFromDate = from.ToString("yyyy/MM/dd");
            string desiredToDate = to.AddDays(1).ToString("yyyy/MM/dd"); // Reason we add one is because toDate is midnight, so to get the last day we need to take the next day at midnight (when it starts)

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.users WHERE created_at BETWEEN '{desiredFromDate}' AND '{desiredToDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                if (graphFormat == CustomGraphFormats.CustomGraphFormat.Days) 
                {
                    GraphDTO? element = gdto.Where(x => x.date == created_at).FirstOrDefault();
                    if (element != null) element.value++;
                }
                else 
                {
                    foreach (var g in gdto)
                    {
                        if (g.date == created_at || g.daysInBetween.Contains(created_at)) g.value++;
                    }
                }
            }

            
            return gdto;
        }

        public async Task<GraphDTO[]> GetCustomTeams(string fromDate, string toDate)
        {
            throw new NotImplementedException();
        }

        public async Task<GraphDTO[]> GetCustomOrganisations(string fromDate, string toDate)
        {
            throw new NotImplementedException();
        }

        public async Task<GraphDTO[]> GetDailyUsers(int daysInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[daysInThePast+1]; // +1 to get WITH today, as datetime.date is midnight

            for (int i = 0; i <= daysInThePast; i++) gdto[i] = new GraphDTO() {date = DateTime.Now.AddDays(-(daysInThePast - i)).Date.ToString("dd/MM/yyyy"), value = 0 };

            string desiredDate = DateTime.Now.AddDays(-daysInThePast).Date.ToString("yyyy/MM/dd");

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.users WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");
                
                GraphDTO? element = gdto.Where(x => x.date == created_at).FirstOrDefault();
                if (element != null) element.value++;
            }

            return gdto;
        }

        public async Task<GraphDTO[]> GetWeeklyUsers(int weeksInThePast) 
        {
            GraphDTO[] gdto = new GraphDTO[weeksInThePast];

            for (int i = 1; i <= weeksInThePast; i++) 
            {
                gdto[i - 1] = new GraphDTO() { date = DateTime.Now.AddDays(-(i * 7)).Date.ToString("dd/MM/yyyy"), value = 0, daysInBetween = new string[6] };

                for (int k = 1; k <= 6; k++) 
                {
                    DateTime date = DateTime.Parse(gdto[i - 1].date);
                    gdto[i - 1].daysInBetween[k-1] = date.AddDays(k).Date.ToString("dd/MM/yyyy");
                } 
            }

            string desiredDate = DateTime.Now.AddDays(-weeksInThePast * 7).Date.ToString("yyyy/MM/dd");

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.users WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                foreach (var g in gdto) 
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at)) g.value++;
                }
            }

            return gdto.Reverse().ToArray();
        }

        public async Task<GraphDTO[]> GetMonthlyUsers(int monthsInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[monthsInThePast];

            for (int i = 0; i < monthsInThePast; i++) 
            {
                DateTime tempToday = DateTime.Now.Date.AddMonths(-i);
                int daysInMonth = DateTime.DaysInMonth(tempToday.Year, tempToday.Month);

                gdto[i] = new GraphDTO() { date = $"01/{tempToday.ToString("MM/yyyy")}", daysInBetween = new string[daysInMonth-1], value = 0 };

                for (int k = 1; k <= daysInMonth-1; k++) 
                {
                    DateTime tempDate = DateTime.Parse(gdto[i].date);
                    gdto[i].daysInBetween[k-1] = tempDate.AddDays(k).ToString("dd/MM/yyyy");
                }
            }

            string desiredDate = $"{DateTime.Now.AddMonths(-monthsInThePast).Date.ToString("yyyy/MM")}/01";

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.users WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                foreach (var g in gdto)
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at)) g.value++;
                }
            }

            return gdto.Reverse().ToArray();
        }

        public async Task<GraphDTO[]> GetDailyTeams(int daysInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[daysInThePast];

            for (int i = 0; i < daysInThePast; i++) gdto[i] = new GraphDTO() { date = DateTime.Now.AddDays(-(daysInThePast - i)).Date.ToString("dd/MM/yyyy"), value = 0 };

            string desiredDate = DateTime.Now.AddDays(-daysInThePast).Date.ToString("yyyy/MM/dd");

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.teams WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                GraphDTO? element = gdto.Where(x => x.date == created_at).FirstOrDefault();
                if (element != null) element.value++;
            }

            return gdto;
        }

        public async Task<GraphDTO[]> GetWeeklyTeams(int weeksInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[weeksInThePast];

            for (int i = 1; i <= weeksInThePast; i++)
            {
                gdto[i - 1] = new GraphDTO() { date = DateTime.Now.AddDays(-(i * 7)).Date.ToString("dd/MM/yyyy"), value = 0, daysInBetween = new string[6] };

                for (int k = 1; k <= 6; k++)
                {
                    DateTime date = DateTime.Parse(gdto[i - 1].date);
                    gdto[i - 1].daysInBetween[k - 1] = date.AddDays(k).Date.ToString("dd/MM/yyyy");
                }
            }

            string desiredDate = DateTime.Now.AddDays(-weeksInThePast * 7).Date.ToString("yyyy/MM/dd");

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.teams WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                foreach (var g in gdto)
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at)) g.value++;
                }
            }

            return gdto.Reverse().ToArray();
        }

        public async Task<GraphDTO[]> GetMonthlyTeams(int monthsInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[monthsInThePast];

            for (int i = 0; i < monthsInThePast; i++)
            {
                DateTime tempToday = DateTime.Now.Date.AddMonths(-i);
                int daysInMonth = DateTime.DaysInMonth(tempToday.Year, tempToday.Month);

                gdto[i] = new GraphDTO() { date = $"01/{tempToday.ToString("MM/yyyy")}", daysInBetween = new string[daysInMonth - 1], value = 0 };

                for (int k = 1; k <= daysInMonth - 1; k++)
                {
                    DateTime tempDate = DateTime.Parse(gdto[i].date);
                    gdto[i].daysInBetween[k - 1] = tempDate.AddDays(k).ToString("dd/MM/yyyy");
                }
            }

            string desiredDate = $"{DateTime.Now.AddMonths(-monthsInThePast).Date.ToString("yyyy/MM")}/01";

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.teams WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                foreach (var g in gdto)
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at))g.value++;
                }
            }

            return gdto.Reverse().ToArray();
        }

        public async Task<GraphDTO[]> GetDailyOrganisations(int daysInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[daysInThePast];

            for (int i = 0; i < daysInThePast; i++) gdto[i] = new GraphDTO() { date = DateTime.Now.AddDays(-(daysInThePast - i)).Date.ToString("dd/MM/yyyy"), value = 0 };

            string desiredDate = DateTime.Now.AddDays(-daysInThePast).Date.ToString("yyyy/MM/dd");

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.organizations WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                GraphDTO? element = gdto.Where(x => x.date == created_at).FirstOrDefault();
                if (element != null) element.value++;
            }

            return gdto;
        }

        public async Task<GraphDTO[]> GetWeeklyOrganisations(int weeksInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[weeksInThePast];

            for (int i = 1; i <= weeksInThePast; i++)
            {
                gdto[i - 1] = new GraphDTO() { date = DateTime.Now.AddDays(-(i * 7)).Date.ToString("dd/MM/yyyy"), value = 0, daysInBetween = new string[6] };

                for (int k = 1; k <= 6; k++)
                {
                    DateTime date = DateTime.Parse(gdto[i - 1].date);
                    gdto[i - 1].daysInBetween[k - 1] = date.AddDays(k).Date.ToString("dd/MM/yyyy");
                }
            }

            string desiredDate = DateTime.Now.AddDays(-weeksInThePast * 7).Date.ToString("yyyy/MM/dd");

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.organizations WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                foreach (var g in gdto)
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at)) g.value++;
                }
            }

            return gdto.Reverse().ToArray();
        }

        public async Task<GraphDTO[]> GetMonthlyOrganisations(int monthsInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[monthsInThePast];

            for (int i = 0; i < monthsInThePast; i++)
            {
                DateTime tempToday = DateTime.Now.Date.AddMonths(-i);
                int daysInMonth = DateTime.DaysInMonth(tempToday.Year, tempToday.Month);

                gdto[i] = new GraphDTO() { date = $"01/{tempToday.ToString("MM/yyyy")}", daysInBetween = new string[daysInMonth - 1], value = 0 };

                for (int k = 1; k <= daysInMonth - 1; k++)
                {
                    DateTime tempDate = DateTime.Parse(gdto[i].date);
                    gdto[i].daysInBetween[k - 1] = tempDate.AddDays(k).ToString("dd/MM/yyyy");
                }
            }

            string desiredDate = $"{DateTime.Now.AddMonths(-monthsInThePast).Date.ToString("yyyy/MM")}/01";

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT created_at FROM public.organizations WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(0).Date.ToString("dd/MM/yyyy");

                foreach (var g in gdto)
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at)) g.value++;
                }
            }

            return gdto.Reverse().ToArray();
        }
    }
}
