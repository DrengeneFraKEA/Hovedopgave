using Hovedopgave.Server.Database;
using Hovedopgave.Server.DTO;
using Hovedopgave.Server.Models;
using Hovedopgave.Server.Utils;
using Npgsql;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hovedopgave.Server.Services
{
    public class GraphService : IGraphService
    {
        public async Task<GraphDTO[]> GetDailyUsers(int daysInThePast)
        {
            GraphDTO[] gdto = new GraphDTO[daysInThePast]; 

            for (int i = 0; i < daysInThePast; i++) gdto[i] = new GraphDTO() {date = DateTime.Now.AddDays(-(daysInThePast - i)).Date.ToString("dd/MM/yyyy"), value = 0 };

            string desiredDate = DateTime.Now.AddDays(-daysInThePast).Date.ToString("yyyy/MM/dd");

            PostgreSQL psql = new PostgreSQL(false);
            await using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

            await using var command = conn.CreateCommand($"SELECT * FROM public.users WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(13).Date.ToString("dd/MM/yyyy");
                
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

            await using var command = conn.CreateCommand($"SELECT * FROM public.users WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(13).Date.ToString("dd/MM/yyyy");
                string[] daysInBetween = new string[7];

                foreach (var g in gdto) 
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at))
                    {
                        g.value++;
                    }
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

            await using var command = conn.CreateCommand($"SELECT * FROM public.users WHERE created_at > '{desiredDate}'");
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string created_at = reader.GetDateTime(13).Date.ToString("dd/MM/yyyy");

                foreach (var g in gdto)
                {
                    if (g.date == created_at || g.daysInBetween.Contains(created_at))
                    {
                        g.value++;
                    }
                }
            }

            return gdto.Reverse().ToArray();
        }
    }
}
