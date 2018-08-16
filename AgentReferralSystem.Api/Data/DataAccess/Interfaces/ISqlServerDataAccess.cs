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
        Task<AgentOutput> GetAgentByIdAsync(int agentId);
        Task AddAgentAsync(Agent agent);
        Task DeleteAgentAsync(int agentId);
    }
}
