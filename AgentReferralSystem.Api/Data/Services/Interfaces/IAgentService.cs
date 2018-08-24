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
    public interface IAgentService
    {
        Task<IEnumerable<Agent>> GetAgentList();
        Task<AgentViewModel> GetAgentViewModelByAgentIdAsync(int agentId);
        Task<IEnumerable<PACReferralType>> GetPACReferralTypesAsync(string search);
        Task<AgentOutput> GetAgentByIdAsync(int agentId);
        Task<IEnumerable<SaleType>> GetSaleTypes();
        Task AddOrUpdateAgentAsync(Agent agent);
        Task DeleteAgentAsync(int agentId);
        Task SaveExportAgentAsync(AgentViewModel model);
    }
}
