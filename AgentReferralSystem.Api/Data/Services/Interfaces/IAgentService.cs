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
    }
}
