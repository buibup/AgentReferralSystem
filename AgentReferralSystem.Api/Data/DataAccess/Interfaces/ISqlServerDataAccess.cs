using AgentReferralSystem.Api.Data.Models;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.DataAccess.Interfaces
{
    public interface ISqlServerDataAccess
    {
        Task<IEnumerable<Agent>> GetAgentList();
        Task<Agent> GetAgentByIdAsync(int agentId);
        Task<IEnumerable<SaleType>> GetSaleTypes();
        Task<IEnumerable<CommissionItem>> GetItemList();
        Task<IEnumerable<int>> GetCustomerByAgentId(int AgentId);
        Task<IEnumerable<CommissionItem>> GetCommissionItemById(int? agentId = null, int? PatientId = null);
        Task SaveCommissionItem(CommissionItem item);
        Task AddOrUpdateAgentAsync(Agent agent);
        Task DeleteAgentAsync(int agentId);
        Task<IEnumerable<RewardItem>> GetRewardList();
        Task<IEnumerable<RewardItemHistory>> GetRewardHistoryList();
        Task<IEnumerable<RewardExchange>> GetRewardExchangeList(int? AgentId = null);
        Task<ScheduleMonthLog> GetScheduleMonthLog();
        Task<IEnumerable<PercentConfig>> GetConfigByCriteria(string ConfigType);
        Task<IEnumerable<PercentConfig>> GetConfigByCriteria(string ConfigType, string ConfigName);
        Task InsertScheduleMonthLog(ScheduleMonthLog Log);
    }
}
