using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.Cache;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using AgentReferralSystem.Api.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Services.Interfaces
{
    public interface IScheduleService
    {
        Task ScheduleMonthlyReward();
        Task MigrateCommissionItem();
        Task RecalDiaryCommission();
        Task<List<Dictionary<string, object>>> TestLog();
    }
}
