using AgentReferralSystem.Api.Data.Models.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Moq
{
    public static class AgentsSaleTypesMoq
    {
        private static List<AgentsSaleTypes> AgentsSaleTypes { get; } = new List<AgentsSaleTypes>()
        {
            new AgentsSaleTypes
            {
                AgentId = 235,
                SaleTypeId = 1,
                BaseCommission = 2.5m,
                Target = 90_000_000,
                TargetPeriod = 2,
                IncreaseIfTargetMet = 0.75m,
                Maximum = 4.75m,
                ResetToBase = 12,
                ApplicableTargetInrease = "subsequent"
            },
            new AgentsSaleTypes
            {
                AgentId = 235,
                SaleTypeId = 2,
                BaseCommission = 6,
                Target = 3_000_000,
                TargetPeriod = 1,
                IncreaseIfTargetMet = 0.75m,
                Maximum = 6.75m,
                ResetToBase = 1,
                ApplicableTargetInrease = "retroactive"
            },
            new AgentsSaleTypes
            {
                AgentId = 235,
                SaleTypeId = 3,
                BaseCommission = 6,
                Target = 3_000_000,
                TargetPeriod = 1,
                IncreaseIfTargetMet = 0.75m,
                Maximum = 6.75m,
                ResetToBase = 1,
                ApplicableTargetInrease = "retroactive"
            },
            new AgentsSaleTypes
            {
                AgentId = 235,
                SaleTypeId = 4,
                BaseCommission = 6,
                Target = 3_000_000,
                TargetPeriod = 1,
                IncreaseIfTargetMet = 0.75m,
                Maximum = 6.75m,
                ResetToBase = 1,
                ApplicableTargetInrease = "retroactive"
            },
            new AgentsSaleTypes
            {
                AgentId = 235,
                SaleTypeId = 5,
                BaseCommission = 6,
                Target = 3_000_000,
                TargetPeriod = 1,
                IncreaseIfTargetMet = 0.75m,
                Maximum = 6.75m,
                ResetToBase = 1,
                ApplicableTargetInrease = "retroactive"
            }
        };

        public static List<AgentsSaleTypes> GetAgentsSaleTypesByAgentId(this int agentId)
        {
            return AgentsSaleTypes.Where(d => d.AgentId == agentId).ToList();
        }
    }
}
