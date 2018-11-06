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
    public interface IRewardService
    {
        Task<IEnumerable<RewardItem>> GetRewardList();
        Task<IEnumerable<RewardItemHistory>> GetRewardHistoryList();
        Task<IEnumerable<RewardExchange>> GetRewardExchangeList(int? AgentId);
        
    }
}
