using AgentReferralSystem.Api.Data.DataAccess.Interfaces;
using AgentReferralSystem.Api.Data.Models.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.DataAccess
{
    public class SqlServerDataAccess : ISqlServerDataAccess
    {
        public Task<Agent> GetAgentById(int agentId)
        {
            throw new NotImplementedException();
        }
    }
}
