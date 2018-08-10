using AgentReferralSystem.Api.Data.Models.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Moq
{
    public static class AgentMoq
    {
        public static Agent GetAgentByAgentId(this int agentId)
        {
            var agent = new Agent()
            {
                AgentId = 235,
                AgentCode = "MT017",
                AgentDesc = "John H. Mramba (Kenya)",
                AgreementDate = DateTime.Now,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddYears(1),
                AgentSaleTypes = 235.GetAgentsSaleTypesByAgentId()
            };

            return agent;
        }
    }
}
