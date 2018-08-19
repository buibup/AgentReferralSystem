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
        Task<AgentViewModel> GetAgentViewModelByAgentIdAsync(int agentId);
        Task<IEnumerable<PACReferralType>> GetPACReferralTypesAsync();
        Task<AgentOutput> GetAgentByIdAsync(int agentId);
        Task AddOrUpdateAgentAsync(Agent agent);
        Task DeleteAgentAsync(int agentId);
        Task SaveExportAgentAsync(AgentViewModel model);
    }
}
