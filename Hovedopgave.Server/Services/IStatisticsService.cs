﻿using Hovedopgave.Server.DTO;

namespace Hovedopgave.Server.Services
{
    public interface IStatisticsService
    {
        Task<SignupStatsDTO> GetSignupStats(DateTime? fromDate, DateTime? toDate);

    }
}
