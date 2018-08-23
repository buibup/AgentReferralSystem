using AgentReferralSystem.Api.Data.Models.Calc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentReferralSystem.Api.Data.Calculate
{
    public static class CommonCalc
    {
        public static decimal SumTargetOfMonth(this AgentCalc agentCalc)
        {
            return agentCalc.Membership.TargetSumMonth + agentCalc.ServiceMember.TargetSumMonth +
                            agentCalc.ServiceNonMember.TargetSumMonth + agentCalc.CompoundingMember.TargetSumMonth +
                            agentCalc.CompoundingNonMember.TargetSumMonth;
        }
    }
}
